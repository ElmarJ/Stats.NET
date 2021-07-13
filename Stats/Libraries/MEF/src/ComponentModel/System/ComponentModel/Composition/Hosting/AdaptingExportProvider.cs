// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using Microsoft.Internal;

namespace System.ComponentModel.Composition.Hosting
{
    public partial class AdaptingExportProvider : ExportProvider, IDisposable
    {
        private object _lock = new object();
        private volatile bool _isDisposed = false;
        private volatile bool _isRunning = false;
        private ExportProvider _sourceProvider;
        private AdapterDefinition[] _adapters;
        private static ImportDefinition _adapterImport = new ContractBasedImportDefinition(CompositionConstants.AdapterContractName,
                                                         (string)null,
                                                         new string[] { CompositionConstants.AdapterFromContractMetadataName,
                                                                        CompositionConstants.AdapterToContractMetadataName },
                                                         ImportCardinality.ZeroOrMore,
                                                         true,                              /* Recomposable */
                                                         false,                            /* Prerequisite */
                                                         CreationPolicy.Any);
        /// <summary>
        ///     Initializes a new instance of the <see cref="AdaptingExportProvider"/> class.
        /// </summary>
        public AdaptingExportProvider()
        {
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (!this._isDisposed)
                {
                    ExportProvider sourceProviderToUnsubscribeFrom = null;
                    try
                    {
                        if (!this._isDisposed)
                        {
                            lock(this._lock)
                            {
                                sourceProviderToUnsubscribeFrom = this._sourceProvider;
                                this._sourceProvider = null;

                                this._isDisposed = true;
                            }
                        }
                    }
                    finally
                    {
                        if (sourceProviderToUnsubscribeFrom != null)
                        {
                            sourceProviderToUnsubscribeFrom.ExportsChanged -= this.OnExportsChangedInternal;
                        }
                    }
                }
            }
        }

        private AdapterDefinition[] Adapters
        {
            get
            {
                if (this._adapters == null)
                {
                    AdapterDefinition[] adapters = this.SourceProvider.GetExports(_adapterImport)
                                        .Select(export => new AdapterDefinition(export))
                                        .ToArray();
                    Thread.MemoryBarrier();

                    lock(this._lock)
                    {
                        if (this._adapters == null)
                        {
                            this._adapters = adapters;
                        }
                    }
                }
                return this._adapters;
            }
        }

        /// <summary>
        ///     Gets the export provider which provides the provider access to additional
        ///     exports.
        /// </summary>
        /// <value>
        ///     The <see cref="ExportProvider"/> which provides the 
        ///     <see cref="AdaptingExportProvider"/> access to additional
        ///     <see cref="Export"/> objects. The default is <see langword="null"/>.
        /// </value>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="value"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     This property has already been set.
        ///     <para>
        ///         -or-
        ///     </para>
        ///     The methods on the <see cref="AdaptingExportProvider"/> 
        ///     have already been accessed.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        ///     The <see cref="AdaptingExportProvider"/> has been disposed of.
        /// </exception>
        /// <remarks>
        ///     This property must be set before accessing any methods on the 
        ///     <see cref="AdaptingExportProvider"/>.
        /// </remarks>
        public ExportProvider SourceProvider
        {
            get
            {
                this.ThrowIfDisposed();

                return this._sourceProvider;
            }
            set
            {
                this.ThrowIfDisposed();

                Requires.NotNull(value, "value");
                lock(this._lock)
                {
                    this.EnsureCanSet(this._sourceProvider);
                    this._sourceProvider = value;
                }

                // This should be safe to do outside the lock, because only the first setter will ever win
                // and others will throw
                this._sourceProvider.ExportsChanged += this.OnExportsChangedInternal;
            }
        }

        private void OnExportsChangedInternal(object sender, ExportsChangedEventArgs e)
        {
            var result = CompositionResult.SucceededResult;

            // Names that are changed due to the fact that adapters have been added/removed
            IEnumerable<string> changedNamesDueToAdaptersChange = Enumerable.Empty<string>();
            if (e.ChangedContractNames.Contains(CompositionConstants.AdapterContractName))
            {
                IEnumerable<AdapterDefinition> oldAdapters = null;
                IEnumerable<AdapterDefinition> newAdapters = null;

                lock(this._lock)
                {
                    oldAdapters = this._adapters ?? Enumerable.Empty<AdapterDefinition>();
                    this._adapters = null;
                }
                newAdapters = this.Adapters;

                // As the contract names that adapter adapts from/to are not considered as 'actual' 
                // imports and exports, we need to manually recompose these when adapters change.                
                changedNamesDueToAdaptersChange = GetChangedContractNames(oldAdapters, newAdapters);
            }

            // names that have been changed due to the actual exports added/removed, with adaptation applied
            IEnumerable<string> adaptedChangedContractNames = this.AdaptContractNames(e.ChangedContractNames);

            // Merge the names together and fire the event
            e = new ExportsChangedEventArgs(changedNamesDueToAdaptersChange.Concat(adaptedChangedContractNames).Distinct().ToArray());
            result = result.MergeResult(CompositionServices.TryInvoke(() => this.OnExportsChanged(e)));
            result.ThrowOnErrors();
        }

        /// <summary>
        /// Returns all exports that match the conditions of the specified import.
        /// </summary>
        /// <param name="definition">The <see cref="ImportDefinition"/> that defines the conditions of the
        /// <see cref="Export"/> to get.</param>
        /// <returns></returns>
        /// <result>
        /// An <see cref="IEnumerable{T}"/> of <see cref="Export"/> objects that match
        /// the conditions defined by <see cref="ImportDefinition"/>, if found; otherwise, an
        /// empty <see cref="IEnumerable{T}"/>.
        /// </result>
        /// <remarks>
        /// 	<note type="inheritinfo">
        /// The implementers should not treat the cardinality-related mismatches as errors, and are not
        /// expected to throw exceptions in those cases.
        /// For instance, if the import requests exactly one export and the provider has no matching exports or more than one,
        /// it should return an empty <see cref="IEnumerable{T}"/> of <see cref="Export"/>.
        /// </note>
        /// </remarks>
        protected override IEnumerable<Export> GetExportsCore(ImportDefinition definition)
        {
            this.ThrowIfDisposed();
            this.EnsureRunning();

            IEnumerable<Export> exports;
            this.SourceProvider.TryGetExports(definition, out exports);

            return this.AdaptExports(exports, definition);
        }

        private IEnumerable<Export> AdaptExports(IEnumerable<Export> exports, ImportDefinition definition)
        {
            Assumes.NotNull(exports, definition);

            IEnumerable<Export> adaptedExports = exports;

            IEnumerable<AdapterDefinition> adapters = this.GetFilteredAdapters(definition);
            if (adapters.Any())
            {   // Avoid getting the constraint unless we actually need to

                adaptedExports = AdaptExportsCore(adapters, exports, definition.GetConstraint());
            }

            return adaptedExports;
        }

        private IEnumerable<Export> AdaptExportsCore(IEnumerable<AdapterDefinition> adapters, IEnumerable<Export> exports, Func<ExportDefinition, bool> constraint)
        {
            var result = CompositionResult.SucceededResult;
            foreach (AdapterDefinition adapter in adapters)
            {
                // We validate the adapter every attempted usage of it, that
                // way if there is an error, then it is raised everytime 
                // someone attempts to request the contract name it adapts to
                result = result.MergeResult(CompositionServices.TryInvoke(() => adapter.EnsureWellFormedAdapter()));
                if (result.Succeeded)
                {
                    CompositionResult<IEnumerable<Export>> adaptedExportsResult = this.GetAdaptedExportsFromAdapter(adapter, constraint);
                    result = result.MergeErrors(adaptedExportsResult.Errors);

                    if (adaptedExportsResult.Succeeded)
                    {
                        exports = exports.Concat(adaptedExportsResult.Value);
                    }
                }
            }

            result.ThrowOnErrors();
            return exports;
        }

        private IEnumerable<string> AdaptContractNames(IEnumerable<string> changedContractNames)
        {
            // When the contract name that an adapter adapts-from changes, 
            // the associated contract name an adapter adapts-to should also be 
            // considered as changed.

            Assumes.NotNull(changedContractNames);

            List<string> newContractNames = new List<string>();
            foreach (AdapterDefinition adapter in this.Adapters)
            {
                foreach (string contractName in changedContractNames)
                {
                    if (StringComparers.ContractName.Equals(adapter.FromContractName, contractName))
                    {
                        newContractNames.Add(adapter.ToContractName);
                    }
                }
            }

            return changedContractNames.Concat(newContractNames);
        }

        private static ImportDefinition CreateAdapterExportsImport(AdapterDefinition adapter)
        {
            return new ContractBasedImportDefinition(adapter.FromContractName, 
                                                     (string)null, 
                                                     (IEnumerable<string>)null,
                                                     ImportCardinality.ZeroOrMore,
                                                     false,     /* Recomposable */
                                                     false,     /* Prerequisite */
                                                     CreationPolicy.Any);    
        }

        private CompositionResult<IEnumerable<Export>> GetAdaptedExportsFromAdapter(AdapterDefinition adapter, Func<ExportDefinition, bool> constraint)
        {
            // Query the container for all exports that the specified adapter 
            // can adapt, then run each one through the adapter, returning the ones 
            // that come out as non-null and also meet the specified constraint

            Assumes.NotNull(adapter, constraint);

            ImportDefinition adapterImport = CreateAdapterExportsImport(adapter);

            CompositionResult<IEnumerable<Export>> exportsToAdaptResult = TryGetExports(this.SourceProvider, adapterImport);
            if (!exportsToAdaptResult.Succeeded)
            {
                return exportsToAdaptResult;
            }

            List<Export> adaptedExports = new List<Export>();            

            IEnumerable<Export> exportsToAdapt = exportsToAdaptResult.Value;
            foreach (Export exportToAdapt in exportsToAdapt)
            {
                CompositionResult<Export> adaptedExportResult = CompositionServices.TryInvoke(() => adapter.Adapt(exportToAdapt));
                if (!adaptedExportResult.Succeeded)
                {
                    // Once we've hit an error, skip this adapter
                    return adaptedExportResult.ToResult<IEnumerable<Export>>();
                }

                Export adaptedExport = adaptedExportResult.Value;
                if (adaptedExport != null && constraint(adaptedExport.Definition))
                {
                    adaptedExports.Add(adaptedExport);
                }
            }

            return new CompositionResult<IEnumerable<Export>>(adaptedExports);
        }

        private static CompositionResult<IEnumerable<Export>> TryGetExports(ExportProvider provider, ImportDefinition definition)
        {
            try
            {
                IEnumerable<Export> exports = provider.GetExports(definition);
                return new CompositionResult<IEnumerable<Export>>(exports);
            }
            catch (CompositionException ex)
            {
                return new CompositionResult<IEnumerable<Export>>(ex.Errors);
            }
        }

        private IEnumerable<AdapterDefinition> GetFilteredAdapters(ImportDefinition definition)
        {
            // If we have access to the contract name, such as pulling it directly from the import,
            // then we can query the adapters based on that, otherwise, we have to return them all.
            ContractBasedImportDefinition contractBasedDefinition = definition as ContractBasedImportDefinition;
            if (contractBasedDefinition == null)
            {
                return this.Adapters;
            }

            return this.Adapters.Where(adapter =>
            {
                return adapter.CanAdaptFrom(contractBasedDefinition.ContractName);
            });
        }

        private static IEnumerable<string> GetChangedContractNames(IEnumerable<AdapterDefinition> oldAdapters, IEnumerable<AdapterDefinition> newAdapters)
        {
            Assumes.NotNull(oldAdapters, newAdapters);

            // Do a symmetric difference to find out what adapters have changed
            IEnumerable<string> removedContractNames = oldAdapters.Where(oldAdapter => !newAdapters.Any(newAdapter => newAdapter.Export == oldAdapter.Export))
                                                                  .Select(oldAdapter => oldAdapter.ToContractName);

            IEnumerable<string> addedContractNames = newAdapters.Where(newAdapter => !oldAdapters.Any(oldAdapter => oldAdapter.Export == newAdapter.Export))
                                                                .Select(newAdapter => newAdapter.ToContractName);

            return removedContractNames.Concat(addedContractNames);
        }

        [DebuggerStepThrough]
        private void ThrowIfDisposed()
        {
            if (this._isDisposed)
            {
                throw ExceptionBuilder.CreateObjectDisposed(this);
            }
        }

        [DebuggerStepThrough]
        private void EnsureCanRun()
        {
            if (this.SourceProvider == null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, Strings.ObjectMustBeInitialized, "SourceProvider")); // NOLOC
            }
        }

        [DebuggerStepThrough]
        private void EnsureRunning()
        {
            if (!this._isRunning)
            {
                lock(this._lock)
                {
                    if (!this._isRunning)
                    {
                        this.EnsureCanRun();
                        this._isRunning = true;
                    }
                }
            }
        }

        [DebuggerStepThrough]
        private void EnsureCanSet<T>(T currentValue)
            where T : class
        {
            if ((this._isRunning) || (currentValue != null))
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, Strings.ObjectAlreadyInitialized));
            }
        }
    }
}
