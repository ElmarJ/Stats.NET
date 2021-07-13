// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Microsoft.Internal;
using Microsoft.Internal.Collections;

namespace System.ComponentModel.Composition.Hosting
{
    public partial class CatalogExportProvider : ExportProvider, IDisposable
    {
        // NOTE : we need to carefully consdier whether we can replace this with a regular CLR lock
        // We only use ReadLock once, to make sure that shared parts are created once.
        // This is only justified if we expect the hits to be much higher than the misses.
        private Lock _lock = new Lock();
        private Dictionary<ComposablePartDefinition, ComposablePart> _activatedSharedParts = new Dictionary<ComposablePartDefinition, ComposablePart>();
        private ConditionalWeakTable<object, List<ComposablePart>> _conditionalReferencesForRecomposableParts = new ConditionalWeakTable<object, List<ComposablePart>>();
        private List<ComposablePart> _partsToDispose = new List<ComposablePart>();
        private ComposablePartCatalog _catalog;
        private volatile bool _isDisposed = false;
        private volatile bool _isRunning = false;
        private ExportProvider _sourceProvider;
        private CompositionEngine _compositionEngine;

        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogExportProvider"/> class.
        /// </summary>
        /// <param name="catalog">
        ///     The <see cref="ComposablePartCatalog"/> that the <see cref="CatalogExportProvider"/>
        ///     uses to produce <see cref="Export"/> objects.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="catalog"/> is <see langword="null"/>.
        /// </exception>
        public CatalogExportProvider(ComposablePartCatalog catalog)
        {
            Requires.NotNull(catalog, "catalog");

            this._catalog = catalog;

            var notifyCatalogChanged = this._catalog as INotifyComposablePartCatalogChanged;
            if (notifyCatalogChanged != null)
            {
                notifyCatalogChanged.Changed += this.OnCatalogChanged;
            }
        }

        /// <summary>
        ///     Gets the composable part catalog that the provider users to 
        ///     produce exports.
        /// </summary>
        /// <value>
        ///     The <see cref="ComposablePartCatalog"/> that the 
        ///     <see cref="CatalogExportProvider"/>
        ///     uses to produce <see cref="Export"/> objects.
        /// </value>
        /// <exception cref="ObjectDisposedException">
        ///     The <see cref="CompositionContainer"/> has been disposed of.
        /// </exception>
        public ComposablePartCatalog Catalog
        {
            get 
            {
                ThrowIfDisposed();

                return _catalog; 
            }
        }

        /// <summary>
        ///     Gets the export provider which provides the provider access to additional
        ///     exports.
        /// </summary>
        /// <value>
        ///     The <see cref="ExportProvider"/> which provides the 
        ///     <see cref="CatalogExportProvider"/> access to additional
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
        ///     The methods on the <see cref="CatalogExportProvider"/> 
        ///     have already been accessed.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        ///     The <see cref="CatalogExportProvider"/> has been disposed of.
        /// </exception>
        /// <remarks>
        ///     This property must be set before accessing any methods on the 
        ///     <see cref="CatalogExportProvider"/>.
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

                using (new WriteLock(this._lock))
                {
                    this.EnsureCanSet(this._sourceProvider);
                    this._sourceProvider = value;
                }

                // This should be safe to do outside the lock, because only the first setter will ever win
                // and others will throw
                CompositionEngine compositionEngine = new CompositionEngine();
                compositionEngine.SourceProvider = _sourceProvider;
                Thread.MemoryBarrier();
                this._compositionEngine = compositionEngine;
            }
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
                    bool disposeLock = false;
                    INotifyComposablePartCatalogChanged catalogToUnsubscribeFrom = null;
                    IEnumerable<ComposablePart> partsToDispose = null;

                    try
                    {
                        using (new WriteLock(this._lock))
                        {
                            if (!this._isDisposed)
                            {
                                catalogToUnsubscribeFrom = this._catalog as INotifyComposablePartCatalogChanged;
                                this._catalog = null;

                                partsToDispose = this._partsToDispose;
                                this._partsToDispose = null;

                                this._activatedSharedParts = null;
                                this._conditionalReferencesForRecomposableParts = null;

                                disposeLock = true;
                                this._isDisposed = true;
                            }
                        }
                    }
                    finally
                    {
                        if (catalogToUnsubscribeFrom != null)
                        {
                            catalogToUnsubscribeFrom.Changed -= this.OnCatalogChanged;
                        }

                        if (partsToDispose != null)
                        {
                            foreach (var part in partsToDispose)
                            {
                                part.Dispose();
                            }                            
                        }

                        if (disposeLock)
                        {
                            this._lock.Dispose();
                        }
                    }
                }
            }
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

            List<Export> exports = new List<Export>();
            foreach (Tuple<ComposablePartDefinition, ExportDefinition> partDefinitionAndExportDefinition in 
                this._catalog.GetExports(definition))
            {
                exports.Add(CreateExport(partDefinitionAndExportDefinition.Item1, 
                                         partDefinitionAndExportDefinition.Item2,
                                         definition));
            }

            return exports;
        }

        private void OnCatalogChanged(object sender, ComposablePartCatalogChangedEventArgs args)
        {
            IEnumerable<string> changedContractNames = args.ChangedDefinitions
                    .SelectMany(part => part.ExportDefinitions)
                    .Select(export => export.ContractName).Distinct();

            this.OnExportsChanged(new ExportsChangedEventArgs(changedContractNames));
        }

        private Export CreateExport(ComposablePartDefinition partDefinition, 
            ExportDefinition exportDefinition, ImportDefinition importDefinition)
        {
            CreationPolicy importPolicy = importDefinition.GetRequiredCreationPolicy();

            return new CatalogExport(this, partDefinition, exportDefinition, importPolicy);
        }

        private ComposablePart GetComposablePart(ComposablePartDefinition partDefinition, bool isSharedPart)
        {
            this.ThrowIfDisposed();
            this.EnsureRunning();

            ComposablePart part;

            if (isSharedPart)
            {
                part = GetSharedPart(partDefinition);
            }
            else
            {
                part = partDefinition.CreatePart();

                if (part.RequiresDisposal)
                {
                    this._partsToDispose.Add(part);
                }
            }

            return part;
        }

        private ComposablePart GetSharedPart(ComposablePartDefinition partDefinition)
        {
            ComposablePart part;
            bool found = false;

            using (new ReadLock(this._lock))
            {
                found = this._activatedSharedParts.TryGetValue(partDefinition, out part);
            }

            if (!found)
            {
                ComposablePart newPart = partDefinition.CreatePart();

                using (new WriteLock(this._lock))
                {
                    found = this._activatedSharedParts.TryGetValue(partDefinition, out part);

                    if (!found)
                    {
                        this._activatedSharedParts.Add(partDefinition, newPart);
                        part = newPart;
                        if (part.RequiresDisposal)
                        {
                            this._partsToDispose.Add(part);
                        }
                    }
                }
            }

            return part;
        }

        private object GetExportedObject(ComposablePart part, ExportDefinition export, bool isSharedPart)
        {
            this.ThrowIfDisposed();
            this.EnsureRunning();

            Assumes.NotNull(part, export);

            object exportedObject = CompositionServices.GetExportedObjectFromComposedPart(this._compositionEngine, part, export);

            // Only hold conditional references for recomposable non-shared parts because we are 
            // already holding strong references to the shared parts.
            if (exportedObject != null && !isSharedPart && part.IsRecomposable())
            {
                SetConditionalReferenceForRecomposablePart(exportedObject, part);
            }

            return exportedObject; 
        }

        private void ReleasePart(object exportedObject, ComposablePart part)
        {
            this.ThrowIfDisposed();
            this.EnsureRunning();

            Assumes.NotNull(part);

            this._compositionEngine.UnregisterForRecomposition(part);

            if (exportedObject != null)
            {
                this._conditionalReferencesForRecomposableParts.Remove(exportedObject);
            }

            if (this._partsToDispose.Remove(part))
            {
                part.Dispose();
            }
        }

        private void SetConditionalReferenceForRecomposablePart(object exportedObject, ComposablePart part)
        {
            Assumes.NotNull(exportedObject, part);

            List<ComposablePart> partList;

            using (new WriteLock(this._lock))
            {
                if (!this._conditionalReferencesForRecomposableParts.TryGetValue(exportedObject, out partList))
                {
                    partList = new List<ComposablePart>();
                    this._conditionalReferencesForRecomposableParts.Add(exportedObject, partList);
                }

                // There is one really obscure case (one part exporting exact value multiple times) where
                // the part may already be in the list but it isn't a scenario that is interesting so 
                // we simply always add. Later if we change this to support more than non-shared we may
                // need to check if the part already exists to pervent adding it multiple times.
                partList.Add(part);
            }
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
            if ((this._sourceProvider == null) || (this._compositionEngine == null))
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, Strings.ObjectMustBeInitialized, "SourceProvider")); // NOLOC
            }
        }

        [DebuggerStepThrough]
        private void EnsureRunning()
        {
            if (!this._isRunning)
            {
                using (new WriteLock(this._lock))
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
