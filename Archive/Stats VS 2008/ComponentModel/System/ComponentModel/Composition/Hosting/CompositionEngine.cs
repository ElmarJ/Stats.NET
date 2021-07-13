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
using Microsoft.Internal;
using Microsoft.Internal.Collections;

namespace System.ComponentModel.Composition.Hosting
{
    // TODO-MT : this class appears to have some thread safety, but that's largely fiction
    // We protect members and collections to prevent basic memory corruption, but the real evil spot here
    // is the re-entrant loop - the state we are manipulating also sits inside the stack, and thus two racing threads can
    // get to the same part at the same time and start partying on this independently, which will lead to really bad things
    // in terms of the composition state of a part. This needs to be addressed if we have a prayer for this to be thread-safe
    public class CompositionEngine : ICompositionService, IDisposable
    {
        private ConditionalWeakTable<ComposablePart, ComposablePartState> _stateManager = new ConditionalWeakTable<ComposablePart, ComposablePartState>();
        private WeakReferenceCollection<ComposablePart> _recomposableParts = new WeakReferenceCollection<ComposablePart>();
        private Stack<ComposablePartState> _partStack = new Stack<ComposablePartState>();
        private ExportProvider _sourceProvider;
        private Lock _lock = new Lock();
        private volatile bool _isDisposed = false;
        private volatile bool _isRunning = false;
        private const int MaximumNumberOfCompositionIterations = 100;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositionEngine"/> class.
        /// </summary>
        public CompositionEngine()
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
                    ExportProvider sourceProviderToUnsubscribeFrom = null;

                    try
                    {
                        using (new WriteLock(this._lock))
                        {
                            if (!this._isDisposed)
                            {
                                sourceProviderToUnsubscribeFrom = this._sourceProvider;
                                this._sourceProvider = null;

                                this._recomposableParts = null;
                                this._partStack = null;
                                this._stateManager = null;

                                disposeLock = true;
                                this._isDisposed = true;
                            }
                        }
                    }
                    finally
                    {
                        // All of this to is safe to do without a lock as only one thread gets to do it
                        if (sourceProviderToUnsubscribeFrom != null)
                        {
                            sourceProviderToUnsubscribeFrom.ExportsChanged -= this.OnExportsChanged;
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
        ///     Gets the export provider which provides the composition engine access to
        ///     exports.
        /// </summary>
        /// <value>
        ///     The <see cref="ExportProvider"/> which provides the 
        ///     <see cref="CompositionEngine"/> access to <see cref="Export"/> objects. 
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
        ///     The methods on the <see cref="CompositionEngine"/> 
        ///     have already been accessed.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        ///     The <see cref="CompositionEngine"/> has been disposed of.
        /// </exception>
        /// <remarks>
        ///     This property must be set before accessing any methods on the 
        ///     <see cref="CompositionEngine"/>.
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
                value.ExportsChanged += this.OnExportsChanged;
            }
        }

        private ComposablePart[] GetAffectedParts(IEnumerable<string> changedContractNames)
        {
            using (new ReadLock(this._lock))
            {
                return this._recomposableParts.AliveItemsToArray().Where(part =>
                {
                    // NOTE - if the import is something rather complex - that is, it doesn't have
                    // a single contract, we will have to mark the whole part for recomposition
                    // Otherwise we will only recompose it if necessary
                    return part.ImportDefinitions.Any(definition =>
                    {
                        if (!definition.IsRecomposable)
                        {
                            return false;
                        }

                        ContractBasedImportDefinition contractDefinition = definition as ContractBasedImportDefinition;
                        if (contractDefinition != null)
                        {
                            return changedContractNames.Contains(contractDefinition.ContractName, StringComparers.ContractName);
                        }

                        return true;
                    });
                }).ToArray();
            }
        }

        private void OnExportsChanged(object sender, ExportsChangedEventArgs e)
        {
            this.ThrowIfDisposed();
            this.EnsureRunning();

            var result = CompositionResult.SucceededResult;

            ComposablePart[] affectedParts = this.GetAffectedParts(e.ChangedContractNames);
            foreach (ComposablePart part in affectedParts)
            {
                result = result.MergeResult(this.TrySatisfyImports(part, true, true));
            }
            result.ThrowOnErrors();
        }

        /// <summary>
        ///     Satisfies the imports of the specified composable part and registering for it for 
        ///     recomposition if requested.
        /// </summary>
        /// <param name="part">
        ///     The <see cref="ComposablePart"/> to set the imports and register for recomposition.
        /// </param>
        /// <param name="registerForRecomposition">
        ///     Indicates whether the part will keep recomposing after the initial import satisfaction
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="part"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="CompositionException">
        ///     An error occurred during composition. <see cref="CompositionException.Errors"/> will
        ///     contain a collection of errors that occurred.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     The <see cref="SourceProvider"/> property was not set.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        ///     The <see cref="CompositionEngine"/> has been disposed of.
        /// </exception>
        public void SatisfyImports(ComposablePart part, bool registerForRecomposition)
        {
            this.ThrowIfDisposed();
            this.EnsureRunning();

            Requires.NotNull(part, "part");

            CompositionResult result = TrySatisfyImports(part, false, registerForRecomposition);
            result.ThrowOnErrors();
        }

        /// <summary>
        ///     Unregisters the specified part from recomposition.
        /// </summary>
        /// <param name="part">
        ///     The <see cref="ComposablePart"/> to unregister from recomposition.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="part"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     The <see cref="SourceProvider"/> property was not set.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        ///     The <see cref="CompositionEngine"/> has been disposed of.
        /// </exception>
        public void UnregisterForRecomposition(ComposablePart part)
        {
            this.ThrowIfDisposed();
            this.EnsureRunning();

            Requires.NotNull(part, "part");

            ComposablePartState partState;

            using (new WriteLock(this._lock))
            {
                if (this._stateManager.TryGetValue(part, out partState))
                {
                    this._stateManager.Remove(part);
                    this._recomposableParts.Remove(part);
                }
            }

            if (partState != null)
            {
                partState.DisposeAllDependencies();
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        private CompositionResult TrySatisfyImports(ComposablePart part, bool recomposeImports, bool registerForRecomposition)
        {
            CompositionResult result = CompositionResult.SucceededResult;
            ComposablePartState partState = this.GetPartState(part);

            try
            {
                int currentStackSize = 0;
                using (new WriteLock(this._lock))
                {
                    this._partStack.Push(partState);
                    currentStackSize = this._partStack.Count;
                }

                if (currentStackSize > CompositionEngine.MaximumNumberOfCompositionIterations)
                {
                    return result.MergeError(
                        ErrorBuilder.ComposeTookTooManyIterations(CompositionEngine.MaximumNumberOfCompositionIterations));
                }

                if (recomposeImports && partState.State == CompositionState.AllImportsSatisfied)
                {
                    Assumes.IsTrue(registerForRecomposition);

                    using (new WriteLock(this._lock))
                    {
                        this._recomposableParts.Remove(part);
                    }
                    partState.State = CompositionState.NoImportsSatisfied;
                }

                while (partState.State != CompositionState.AllImportsSatisfied)
                {
                    switch (partState.State)
                    {
                        case CompositionState.NoImportsSatisfied:
                            {
                                partState.State = CompositionState.PreExportImportsSatisfying;

                                var preImports = part.ImportDefinitions.Where(import => import.IsPrerequisite);

                                if (recomposeImports)
                                {
                                    // It would be nice to actually we could actually filter this list to 
                                    // only imports that are recomposable and who have matching exports that
                                    // have changed but at this point the design doesn't easily support that.
                                    preImports = preImports.Where(import => import.IsRecomposable);
                                }

                                result = result.MergeResult(
                                    this.TrySatisfyImports(partState, part, preImports));

                                if (!result.Succeeded)
                                {
                                    partState.State = CompositionState.NoImportsSatisfied;
                                    return result;
                                }
                                partState.State = CompositionState.PreExportImportsSatisfied;
                                break;
                            }
                        case CompositionState.PreExportImportsSatisfying:
                            {
                                return result.MergeError(
                                            ErrorBuilder.CreatePartCycle(part));
                            }
                        case CompositionState.PreExportImportsSatisfied:
                            {
                                partState.State = CompositionState.PostExportImportsSatisfying;

                                var postImports = part.ImportDefinitions.Where(import => !import.IsPrerequisite);

                                if (recomposeImports)
                                {
                                    // It would be nice to actually we could actually filter this list to 
                                    // only imports that are recomposable and who have matching exports that
                                    // have changed but at this point the design doesn't easily support that.
                                    postImports = postImports.Where(import => import.IsRecomposable);
                                }

                                result = result.MergeResult(
                                    this.TrySatisfyImports(partState, part, postImports));

                                if (!result.Succeeded)
                                {
                                    partState.State = CompositionState.PreExportImportsSatisfied;
                                    return result;
                                }
                                partState.State = CompositionState.PostExportImportsSatisfied;
                                break;
                            }
                        case CompositionState.PostExportImportsSatisfying:
                            {
                                // Ensure that no import in the cycle requires fully composed.
                                if (!this.AllPartsInCycleSatisfyCondition(partState, s => !s.RequiresFullyComposed))
                                {
                                    return result.MergeError(
                                                ErrorBuilder.CreatePartCycle(part));
                                }

                                // If no one in the cycle requires fully composed then simply return and allow 
                                // composition to continue. 
                                return result;
                            }
                        case CompositionState.PostExportImportsSatisfied:
                            {
                                result = result.MergeResult(TryOnComposed(part));

                                if (registerForRecomposition && part.IsRecomposable())
                                {
                                    partState.DisposeOldDependencies();
                                    using (new WriteLock(this._lock))
                                    {
                                        this._recomposableParts.Add(part);
                                    }
                                }
                                partState.State = CompositionState.AllImportsSatisfied;
                                break;
                            }
                    }
                }
            }
            finally
            {
                using (new WriteLock(this._lock))
                {
                    ComposablePartState topState = this._partStack.Pop();
                    Assumes.IsTrue(partState == topState);
                }
            }

            return result;
        }

        private CompositionResult TrySatisfyImports(ComposablePartState partState, ComposablePart part, IEnumerable<ImportDefinition> definitions)
        {
            CompositionResult result = CompositionResult.SucceededResult;

            foreach (ImportDefinition definition in definitions)
            {
                // Prerequisite's must be always fully composed
                partState.RequiresFullyComposed = definition.IsPrerequisite;

                CompositionResult<IEnumerable<Export>> valueResult = TryGetExports(this._sourceProvider, part, definition);
                result = result.MergeResult(valueResult.ToResult());

                if (!valueResult.Succeeded)
                {
                    continue;
                }

                result = result.MergeResult(TrySetImport(part, definition, valueResult.Value));
                partState.StoreDependencies(definition, valueResult.Value);
            }

            partState.RequiresFullyComposed = false;
            return result;
        }

        private static CompositionResult<IEnumerable<Export>> TryGetExports(ExportProvider provider, ComposablePart part, ImportDefinition definition)
        {
            try
            {
                IEnumerable<Export> exports = provider.GetExports(definition);

                return new CompositionResult<IEnumerable<Export>>(exports);
            }
            catch (CompositionException ex)
            {   // Composing one of the definition's dependencies failed

                return new CompositionResult<IEnumerable<Export>>(
                   ErrorBuilder.CreatePartCannotSetImport(part, definition, ex));
            }
            catch (ImportCardinalityMismatchException ex)
            {   // Either not enough or too many exports that match the definition

                CompositionException exception = new CompositionException(ErrorBuilder.CreateImportCardinalityMismatch(ex, definition));

                return new CompositionResult<IEnumerable<Export>>(
                    ErrorBuilder.CreatePartCannotSetImport(part, definition, exception));
            }
        }

        private static CompositionResult TrySetImport(ComposablePart part, ImportDefinition definition, IEnumerable<Export> exports)
        {
            try
            {
                part.SetImport(definition, exports);
                return CompositionResult.SucceededResult;
            }
            catch (CompositionException ex)
            {   // Pulling on one of the exports failed

                return new CompositionResult(
                    ErrorBuilder.CreatePartCannotSetImport(part, definition, ex));
            }
            catch (ComposablePartException ex)
            {   // Type mismatch between export and import

                return new CompositionResult(
                    ErrorBuilder.CreatePartCannotSetImport(part, definition, ex));
            }
        }

        private static CompositionResult TryOnComposed(ComposablePart part)
        {
            try
            {
                part.OnComposed();
                return CompositionResult.SucceededResult;
            }
            catch (ComposablePartException ex)
            {   // Type failed to be constructed, imports could not be set, etc

                return new CompositionResult(
                    ErrorBuilder.CreatePartCannotActivate(part, ex));
            }
        }

        private ComposablePartState GetPartState(ComposablePart composablePart)
        {
            ComposablePartState partState;
            bool found = false;

            using (new ReadLock(this._lock))
            {
                found = this._stateManager.TryGetValue(composablePart, out partState);
            }

            if (!found)
            {
                ComposablePartState newPartState = new ComposablePartState();

                using (new WriteLock(this._lock))
                {
                    found = this._stateManager.TryGetValue(composablePart, out partState);

                    if (!found)
                    {
                        this._stateManager.Add(composablePart, newPartState);
                        partState = newPartState;
                    }
                }
            }

            return partState;
        }

        private bool AllPartsInCycleSatisfyCondition(ComposablePartState state, Predicate<ComposablePartState> condition)
        {
            Assumes.NotNull(state);
            Assumes.IsTrue(this._partStack.First() == state);

            ComposablePartState lastState = null;

            foreach (ComposablePartState partState in this._partStack.Skip(1))
            {
                if (!condition(partState))
                {
                    return false;
                }

                if (partState == state)
                {
                    lastState = partState;
                    break;
                }
            }

            Assumes.IsTrue(lastState == state);
            return true;
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
            if (this._sourceProvider == null)
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

        private enum CompositionState
        {
            NoImportsSatisfied,
            PreExportImportsSatisfying,
            PreExportImportsSatisfied,
            PostExportImportsSatisfying,
            PostExportImportsSatisfied,
            AllImportsSatisfied
        }

        private class ComposablePartState
        {
            public CompositionState State { get; set; }
            public bool RequiresFullyComposed { get; set; }
            private Dictionary<ImportDefinition, List<IDisposable>> _importedValues;
            private List<IDisposable> _oldDependencies;

            private List<IDisposable> OldDependencies
            {
                get
                {
                    if (_oldDependencies == null)
                    {
                        this._oldDependencies = new List<IDisposable>();
                    }
                    return this._oldDependencies;
                }
            }

            private Dictionary<ImportDefinition, List<IDisposable>> ImportedValues
            {
                get
                {
                    if (this._importedValues == null)
                    {
                        this._importedValues = new Dictionary<ImportDefinition, List<IDisposable>>();
                    }
                    return this._importedValues;
                }
            }

            public void StoreDependencies(ImportDefinition definition, IEnumerable<Export> exports)
            {
                List<IDisposable> exportReferences;

                if (this.ImportedValues.TryGetValue(definition, out exportReferences))
                {
                    this.OldDependencies.AddRange(exportReferences);
                }
                exportReferences = new List<IDisposable>();
                exportReferences.AddRange(exports.OfType<IDisposable>());
                this.ImportedValues[definition] = exportReferences;
            }

            public void DisposeAllDependencies()
            {
                if (this._importedValues != null)
                {
                    IEnumerable<IDisposable> dependencies = this._importedValues.Values
                        .SelectMany(exports => exports);

                    this._importedValues = null;

                    dependencies.ForEach(disposable => disposable.Dispose());
                }
            }

            public void DisposeOldDependencies()
            {
                if (this._oldDependencies != null)
                {
                    IEnumerable<IDisposable> dependencies = this._oldDependencies;
                    this._oldDependencies = null;

                    dependencies.ForEach(disposable => disposable.Dispose());
                }
            }
        }
    }
}
