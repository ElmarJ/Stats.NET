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
    public class ComposablePartExportProvider : ExportProvider, IDisposable
    {
        private List<ComposablePart> _parts = new List<ComposablePart>();
        private volatile bool _isDisposed = false;
        private volatile bool _isRunning = false;
        private Lock _lock = new Lock();
        private ExportProvider _sourceProvider;
        private CompositionEngine _compositionEngine;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComposablePartExportProvider"/> class.
        /// </summary>
        public ComposablePartExportProvider()
        {
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
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
                    CompositionEngine compositionEngine = null;
                    try
                    {
                        using (new WriteLock(this._lock))
                        {
                            if (!this._isDisposed)
                            {
                                compositionEngine = this._compositionEngine;
                                this._compositionEngine = null;
                                this._sourceProvider = null;
                                this._isDisposed = true;
                                disposeLock = true;
                            }
                        }
                    }
                    finally
                    {
                        if (compositionEngine != null)
                        {
                            compositionEngine.Dispose();
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
        ///     Gets the export provider which provides the provider access to
        ///     exports.
        /// </summary>
        /// <value>
        ///     The <see cref="ExportProvider"/> which provides the 
        ///     <see cref="ComposablePartExportProvider"/> access to <see cref="Export"/> objects. 
        ///     The default is <see langword="null"/>.
        /// </value>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="value"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     This property has already been set.
        ///     <para>
        ///         -or-
        ///     </para>
        ///     The methods on the <see cref="ComposablePartExportProvider"/> 
        ///     have already been accessed.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        ///     The <see cref="ComposablePartExportProvider"/> has been disposed of.
        /// </exception>
        /// <remarks>
        ///     This property must be set before accessing any methods on the 
        ///     <see cref="ComposablePartExportProvider"/>.
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

            // We never change the list of parts and exports in place, but rather copy, change and write.
            // therefore all we need to do here is to read the member
            List<ComposablePart> parts = null;
            using (new ReadLock(this._lock))
            {
                parts = this._parts;
            }

            if (parts.Count == 0)
            {
                return Enumerable.Empty<Export>();
            }

            return GetExportsCore(parts, definition.GetConstraint());
        }

        private IEnumerable<Export> GetExportsCore(IEnumerable<ComposablePart> parts, Func<ExportDefinition, bool> constraint)
        {
            Assumes.NotNull(parts, constraint);

            List<Export> exports = new List<Export>();
            foreach (var part in parts)
            {
                foreach (var exportDefinition in part.ExportDefinitions)
                {
                    if (constraint(exportDefinition))
                    {
                        exports.Add(this.CreateExport(part, exportDefinition));
                    }
                }
            }
            return exports;
        }       

        public void Compose(CompositionBatch batch)
        {
            this.ThrowIfDisposed();
            this.EnsureRunning();

            Requires.NotNull(batch, "batch");

            CompositionResult result = CompositionResult.SucceededResult;

            // Clone the batch, so that the external changes wouldn't hapen half-way thorugh compose
            // NOTE : this does not guarantee the atmicity of cloning, which is not the goal anyway, 
            // rather the fact that all calls will deal with the same - for all intents an purposes - immutable batch
            batch = new CompositionBatch(batch.PartsToAdd, batch.PartsToRemove);

            if ((batch.PartsToAdd.Count == 0) && (batch.PartsToRemove.Count == 0))
            {
                return;
            }

            // Update Parts
            // - Unregister any removed component parts
            // - Register any added component parts
            // - Recompose any imports effected by the these changes
            result = result.MergeResult(this.TryUpdateParts(batch));

            // Satisfy Imports
            // - Satisfy imports on all newly added component parts
            result = result.MergeResult(this.TrySatisfyImports(batch));

            // return errors
            result.ThrowOnErrors();
        }

        private CompositionResult TryUpdateParts(CompositionBatch batch)
        {
            Assumes.NotNull(batch);

            CompositionResult result = CompositionResult.SucceededResult;

            // Copy the current list of parts - we are about to modify it
            // This is an OK thing to do as this is the only method that can modify the List AND Compose can
            // only be executed on one thread at a time - thus two different threads cannot tramp over each other
            List<ComposablePart> parts = null;
            using (new ReadLock(this._lock))
            {
                parts = this._parts.ToList(); // this copies the list
            }

            // Unregister any removed component parts
            foreach (ComposablePart part in batch.PartsToRemove)
            {
                parts.Remove(part);
                this._compositionEngine.UnregisterForRecomposition(part);
            }

            foreach (ComposablePart part in batch.PartsToAdd)
            {
                parts.Add(part);
            }

            // Recompose any imports effected by the these changes
            IEnumerable<string> changedContractNames = batch.PartsToAdd.Concat(batch.PartsToRemove).
                SelectMany(part => part.ExportDefinitions).
                Select(exportDefinition => exportDefinition.ContractName).
                Distinct().ToArray();

            // now replace the old parts
            using (new WriteLock(this._lock))
            {
                this._parts = parts;
            }

            // at this point the changes are observable through GetExports, thus we can fire the event
            result = result.MergeResult(
                CompositionServices.TryInvoke(() => this.OnExportsChanged(new ExportsChangedEventArgs(changedContractNames)))
                );

            return result;
        }

        private Export CreateExport(ComposablePart part, ExportDefinition export)
        {
            return new Export(export, () => GetExportedObject(part, export));
        }

        private object GetExportedObject(ComposablePart part, ExportDefinition export)
        {
            this.ThrowIfDisposed();
            this.EnsureRunning();

            return CompositionServices.GetExportedObjectFromComposedPart(this._compositionEngine, part, export);
        }

        private CompositionResult TrySatisfyImports(CompositionBatch batch)
        {
            Assumes.NotNull(batch);
            var result = CompositionResult.SucceededResult;

            foreach (ComposablePart part in batch.PartsToAdd)
            {
                result = result.MergeResult(CompositionServices.TryInvoke(() => this._compositionEngine.SatisfyImports(part, true)));
            }

            return result;
        }

        [DebuggerStepThrough]
        private void ThrowIfDisposed()
        {
            if (this._isDisposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
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
