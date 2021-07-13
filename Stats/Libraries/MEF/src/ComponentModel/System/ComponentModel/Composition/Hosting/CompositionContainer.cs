// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition.Primitives;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Internal;
using Microsoft.Internal.Collections;
using System.Threading;

namespace System.ComponentModel.Composition.Hosting
{
    public partial class CompositionContainer : ExportProvider, ICompositionService, IDisposable
    {
        private CompositionEngine _compositionEngine;
        private ComposablePartExportProvider _partExportProvider;
        private AggregateExportProvider _aggregatingExportProvider;
        private AdaptingExportProvider _rootProvider;
        private CatalogExportProvider _catalogExportProvider;
        private readonly ReadOnlyCollection<ExportProvider> _providers;
        private volatile int _isDisposed = 0;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CompositionContainer"/> class.
        /// </summary>
        public CompositionContainer()
            : this((ComposablePartCatalog)null)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="CompositionContainer"/> class 
        ///     with the specified export providers.
        /// </summary>
        /// <param name="providers">
        ///     A <see cref="Array"/> of <see cref="ExportProvider"/> objects which provide 
        ///     the <see cref="CompositionContainer"/> access to <see cref="Export"/> objects,
        ///     or <see langword="null"/> to set <see cref="Providers"/> to an empty
        ///     <see cref="ReadOnlyCollection{T}"/>.
        /// </param>
        /// <exception cref="ArgumentException">
        ///     <paramref name="providers"/> contains an element that is <see langword="null"/>.
        /// </exception>
        public CompositionContainer(params ExportProvider[] providers) : 
            this((ComposablePartCatalog)null, providers)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="CompositionContainer"/> class 
        ///     with the specified catalog and export providers.
        /// </summary>
        /// <param name="providers">
        ///     A <see cref="Array"/> of <see cref="ExportProvider"/> objects which provide 
        ///     the <see cref="CompositionContainer"/> access to <see cref="Export"/> objects,
        ///     or <see langword="null"/> to set <see cref="Providers"/> to an empty 
        ///     <see cref="ReadOnlyCollection{T}"/>.
        /// </param>
        /// <exception cref="ArgumentException">
        ///     <paramref name="providers"/> contains an element that is <see langword="null"/>.
        /// </exception>
        public CompositionContainer(ComposablePartCatalog catalog, params ExportProvider[] providers)
        {
            Requires.NullOrNotNullElements(providers, "providers");

            this._compositionEngine = new CompositionEngine();
            this._compositionEngine.SourceProvider = this;
            this._partExportProvider = new ComposablePartExportProvider();
            this._partExportProvider.SourceProvider = this;
            this._providers = new ReadOnlyCollection<ExportProvider>((ExportProvider[])providers.Clone() ?? new ExportProvider[0]);

            if (catalog != null)
            {
                this._catalogExportProvider = new CatalogExportProvider(catalog);
                this._catalogExportProvider.SourceProvider = this;
            }

            this._rootProvider = new AdaptingExportProvider();
            this._rootProvider.ExportsChanged += this.OnExportsChangedInternal;

            providers = new ExportProvider[] { this._partExportProvider, this._catalogExportProvider }.Concat(this._providers).WhereNotNull().ToArray();

            // we only build the aggregating provider if necessary - that is, if we have more than one provider to aggregate
            if (providers.Length > 1)
            {
                this._aggregatingExportProvider = new AggregateExportProvider(providers);
                this._rootProvider.SourceProvider = this._aggregatingExportProvider;
            }
            else
            {
                Assumes.IsTrue(providers.Length == 1);
                this._rootProvider.SourceProvider = providers[0];
            }
        }

        /// <summary>
        ///     Gets the catalog which provides the container access to exports produced
        ///     from composable parts.
        /// </summary>
        /// <value>
        ///     The <see cref="ComposablePartCatalog"/> which provides the 
        ///     <see cref="CompositionContainer"/> access to exports produced from
        ///     <see cref="ComposablePart"/> objects. The default is <see langword="null"/>.
        /// </value>
        /// <exception cref="ObjectDisposedException">
        ///     The <see cref="CompositionContainer"/> has been disposed of.
        /// </exception>
        public ComposablePartCatalog Catalog
        {
            get 
            {
                ThrowIfDisposed();

                if (_catalogExportProvider != null)
                {
                    return _catalogExportProvider.Catalog;
                }

                return null;
            }
        }

        /// <summary>
        ///     Gets the export providers which provide the container access to additional exports.
        /// </summary>
        /// <value>
        ///     A <see cref="ReadOnlyCollection{T}"/> of <see cref="ExportProvider"/> objects
        ///     which provide the <see cref="CompositionContainer"/> access to additional
        ///     <see cref="Export"/> objects. The default is an empty 
        ///     <see cref="ReadOnlyCollection{T}"/>.
        /// </value>
        /// <exception cref="ObjectDisposedException">
        ///     The <see cref="CompositionContainer"/> has been disposed of.
        /// </exception>
        public ReadOnlyCollection<ExportProvider> Providers
        {
            get
            {
                this.ThrowIfDisposed();

                return this._providers;
            }
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
                // NOTE : According to http://msdn.microsoft.com/en-us/library/4bw5ewxy.aspx, the warning is bogus when used with Interlocked API.
#pragma warning disable 420
                if (Interlocked.CompareExchange(ref this._isDisposed, 1, 0) == 0)
#pragma warning restore 420
                {
                        this._rootProvider.ExportsChanged -= this.OnExportsChangedInternal;
                        this._rootProvider.Dispose();
                        if (this._aggregatingExportProvider != null)
                        {
                            this._aggregatingExportProvider.Dispose();
                        }
                        this._partExportProvider.Dispose();
                        if (this._catalogExportProvider != null)
                        {
                            this._catalogExportProvider.Dispose();
                        }
                        this._compositionEngine.Dispose();   
                }
            }
        }
  
        public void Compose(CompositionBatch batch)
        {
            Requires.NotNull(batch, "batch");

            this.ThrowIfDisposed();
            this._partExportProvider.Compose(batch);
        }

        /// <summary>
        ///     Releases the <see cref="Export"/> from the <see cref="CompositionContainer"/>. The behavior
        ///     may vary depending on the implementation of the <see cref="ExportProvider"/> that produced 
        ///     the <see cref="Export"/> instance. As a general rule non shared exports should be early 
        ///     released causing them to be detached from the container.
        ///
        ///     For example the <see cref="CatalogExportProvider"/> will only release 
        ///     an <see cref="Export"/> if it comes from a <see cref="ComposablePart"/> that was constructed
        ///     under a <see cref="CreationPolicy.NonShared" /> context. Release in this context means walking
        ///     the dependency chain of the <see cref="Export"/>s, detaching references from the container and 
        ///     calling Dispose on the <see cref="ComposablePart"/>s as needed. If the <see cref="Export"/> 
        ///     was constructed under a <see cref="CreationPolicy.Shared" /> context the 
        ///     <see cref="CatalogExportProvider"/> will do nothing as it may be in use by other requestors. 
        ///     Those will only be detached when the container is itself disposed.
        /// </summary>
        /// <param name="export"><see cref="Export"/> that needs to be released.</param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="export"/> is <see langword="null"/>.
        /// </exception>
        [SuppressMessage("Microsoft.Performance", "CA1822")]
        public void ReleaseExport(Export export)
        {
            Requires.NotNull(export, "export");

            IDisposable dependency = export as IDisposable;

            if (dependency != null)
            {
                dependency.Dispose();
            }
        }

        /// <summary>
        ///     Releases a set of <see cref="Export"/>s from the <see cref="CompositionContainer"/>. 
        ///     See also <see cref="ReleaseExport"/>.
        /// </summary>
        /// <param name="exports"><see cref="Export"/>s that need to be released.</param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="exports"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <paramref name="exports"/> contains an element that is <see langword="null"/>.
        /// </exception>
        public void ReleaseExports(IEnumerable<Export> exports)
        {
            Requires.NotNullOrNullElements(exports, "exports");

            foreach (Export export in exports)
            {
                ReleaseExport(export);
            }
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
        /// <exception cref="ObjectDisposedException">
        ///     The <see cref="CompositionContainer"/> has been disposed of.
        /// </exception>
        public void SatisfyImports(ComposablePart part, bool registerForRecomposition)
        {
            this.ThrowIfDisposed();
            this._compositionEngine.SatisfyImports(part, registerForRecomposition);
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
        /// <exception cref="ObjectDisposedException">
        ///     The <see cref="CompositionContainer"/> has been disposed of.
        /// </exception>
        public void UnregisterForRecomposition(ComposablePart part)
        {
            this.ThrowIfDisposed();
            this._compositionEngine.UnregisterForRecomposition(part);
        }

        internal void OnExportsChangedInternal(object sender, ExportsChangedEventArgs e)
        {
            this.OnExportsChanged(e);
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

            IEnumerable<Export> exports = null;
            this._rootProvider.TryGetExports(definition, out exports);

            return exports;
        }

        [DebuggerStepThrough]
        private void ThrowIfDisposed()
        {
            if (this._isDisposed == 1)
            {
                throw ExceptionBuilder.CreateObjectDisposed(this);
            }
        }
    }
}