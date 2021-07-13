﻿// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;

namespace System.ComponentModel.Composition.Primitives
{
    /// <summary>
    ///     Defines the <see langword="abstract"/> base class for composable parts, which 
    ///     import and produce exported objects.
    /// </summary>
    public abstract class ComposablePart : IDisposable
    {
        private bool _isDisposed;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ComposablePart"/> class.
        /// </summary>
        protected ComposablePart()
        {
        }

        /// <summary>
        ///     Gets the export definitions that describe the exported objects provided by the part.
        /// </summary>
        /// <value>
        ///     An <see cref="IEnumerable{T}"/> of <see cref="ExportDefinition"/> objects describing
        ///     the exported objects provided by the <see cref="ComposablePart"/>.
        /// </value>
        /// <exception cref="ObjectDisposedException">
        ///     The <see cref="ComposablePart"/> has been disposed of.
        /// </exception>
        /// <remarks>
        ///     <para>
        ///         <note type="inheritinfo">
        ///             If the <see cref="ComposablePart"/> was created from a 
        ///             <see cref="ComposablePartDefinition"/>, this property should return the result of 
        ///             <see cref="ComposablePartDefinition.ExportDefinitions"/>.
        ///         </note>
        ///      </para>
        ///      <para>
        ///         <note type="inheritinfo">
        ///             Overriders of this property should never return <see langword="null"/>.
        ///             If the <see cref="ComposablePart"/> does not have exports, return an empty 
        ///             <see cref="IEnumerable{T}"/> instead.
        ///         </note>
        ///     </para>
        /// </remarks>
        public abstract IEnumerable<ExportDefinition> ExportDefinitions { get; }

        /// <summary>
        ///     Gets the import definitions that describe the imports required by the part.
        /// </summary>
        /// <value>
        ///     An <see cref="IEnumerable{T}"/> of <see cref="ImportDefinition"/> objects describing
        ///     the imports required by the <see cref="ComposablePart"/>.
        /// </value>
        /// <exception cref="ObjectDisposedException">
        ///     The <see cref="ComposablePart"/> has been disposed of.
        /// </exception>
        /// <remarks>
        ///     <para>
        ///         <note type="inheritinfo">
        ///             If the <see cref="ComposablePart"/> was created from a 
        ///             <see cref="ComposablePartDefinition"/>, this property should return the result of 
        ///             <see cref="ComposablePartDefinition.ImportDefinitions"/>.
        ///         </note>
        ///      </para>
        ///      <para>
        ///         <note type="inheritinfo">
        ///             Overrides of this property should never return <see langword="null"/>.
        ///             If the <see cref="ComposablePart"/> does not have imports, return an empty 
        ///             <see cref="IEnumerable{T}"/> instead.
        ///         </note>
        ///     </para>
        /// </remarks>
        public abstract IEnumerable<ImportDefinition> ImportDefinitions { get; }

        /// <summary>
        ///     Gets the metadata of the part.
        /// </summary>
        /// <value>
        ///     An <see cref="IDictionary{TKey, TValue}"/> containing the metadata of the 
        ///     <see cref="ComposablePart"/>. The default is an empty, read-only
        ///     <see cref="IDictionary{TKey, TValue}"/>.
        /// </value>
        /// <exception cref="ObjectDisposedException">
        ///     The <see cref="ComposablePart"/> has been disposed of.
        /// </exception>
        /// <remarks>
        ///     <para>
        ///         <note type="inheritinfo">
        ///             If the <see cref="ComposablePart"/> was created from a 
        ///             <see cref="ComposablePartDefinition"/>, this property should return the result of 
        ///             <see cref="ComposablePartDefinition.Metadata"/>.
        ///         </note>
        ///      </para>
        ///      <para>
        ///         <note type="inheritinfo">
        ///             Overriders of this property should return a read-only
        ///             <see cref="IDictionary{TKey, TValue}"/> object with a case-sensitive, 
        ///             non-linguistic comparer, such as <see cref="StringComparer.Ordinal"/>, 
        ///             and should never return <see langword="null"/>. If the 
        ///             <see cref="ComposablePart"/> does not contain metadata, return an 
        ///             empty <see cref="IDictionary{TKey, TValue}"/> instead.
        ///         </note>
        ///      </para>
        /// </remarks>
        public virtual IDictionary<string, object> Metadata
        {
            get 
            {
                ThrowIfDisposed();
                return MetadataServices.EmptyMetadata; 
            }
        }

        /// <summary>
        ///     Releases all resources used by the <see cref="ComposablePart"/>.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Called by the composition engine when all required imports on the part have been
        ///     satisfied.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        ///     The <see cref="ComposablePart"/> has been disposed of.
        /// </exception>
        /// <exception cref="ComposablePartException">
        ///     An error occurred activating the <see cref="ComposablePart"/>.
        /// </exception>
        public virtual void OnComposed()
        {
            ThrowIfDisposed();
        }

        /// <summary>
        ///     Gets the exported object described by the specified definition.
        /// </summary>
        /// <param name="definition">
        ///     One of the <see cref="ExportDefinition"/> objects from the 
        ///     <see cref="ExportDefinitions"/> property describing the exported object
        ///     to return.
        /// </param>
        /// <returns>
        ///     The exported <see cref="Object"/> described by <paramref name="definition"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="definition"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <paramref name="definition"/> did not originate from the <see cref="ExportDefinitions"/>
        ///     property on the <see cref="ComposablePart"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     One or more pre-requisite imports, indicated by <see cref="ImportDefinition.IsPrerequisite"/>,
        ///     have not been set.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        ///     The <see cref="ComposablePart"/> has been disposed of.
        /// </exception>
        /// <exception cref="ComposablePartException">
        ///     An error occurred getting the exported object described by the <see cref="ExportDefinition"/>.
        /// </exception>
        public abstract object GetExportedObject(ExportDefinition definition);

        /// <summary>
        ///     Sets the import described by the specified definition with the specified exports.
        /// </summary>
        /// <param name="definition">
        ///     One of the <see cref="ImportDefinition"/> objects from the 
        ///     <see cref="ImportDefinitions"/> property describing the import to be set.
        /// </param>
        /// <param name="exports">
        ///     An <see cref="IEnumerable{T}"/> of <see cref="Export"/> objects of which 
        ///     to set the import described by <paramref name="definition"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="definition"/> is <see langword="null"/>.
        ///     <para>
        ///         -or-
        ///     </para>
        ///     <paramref name="exports"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <paramref name="definition"/> did not originate from the <see cref="ImportDefinitions"/>
        ///     property on the <see cref="ComposablePart"/>.
        ///     <para>
        ///         -or-
        ///     </para>
        ///     <paramref name="exports"/> contains an element that is <see langword="null"/>.
        ///     <para>
        ///         -or-
        ///     </para>
        ///     <paramref name="exports"/> is empty and <see cref="ImportDefinition.Cardinality"/> is 
        ///     <see cref="ImportCardinality.ExactlyOne"/>.
        ///     <para>
        ///         -or-
        ///     </para>
        ///     <paramref name="exports"/> contains more than one element and 
        ///     <see cref="ImportDefinition.Cardinality"/> is <see cref="ImportCardinality.ZeroOrOne"/> or 
        ///     <see cref="ImportCardinality.ExactlyOne"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     <see cref="OnComposed"/> has been previously called and 
        ///     <see cref="ImportDefinition.IsRecomposable"/> is <see langword="false"/>.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        ///     The <see cref="ComposablePart"/> has been disposed of.
        /// </exception>
        /// <exception cref="ComposablePartException">
        ///     An error occurred setting the import described by the <see cref="ImportDefinition"/>.
        /// </exception>
        public abstract void SetImport(ImportDefinition definition, IEnumerable<Export> exports);

        /// <summary>
        ///     Determines if this <see cref="ComposablePart"/> requires its dispose method called. This
        ///     is to help prevent memory bloat by the <see cref="CompositionContainer"/> because it
        ///     doesn't need to hold strong references to <see cref="ComposablePart"/> that aren't disposable.
        /// 
        ///     The value of this property should be constant throughout the lifetime of the part because some
        ///     code such as the <see cref="CompositionContainer"/> will use it to determine if it should
        ///     hold a reference to this part for disposal.
        /// </summary>
        /// <returns>
        ///     <see langword="true"/> if this part should have its dispose called;
        ///     otherwise <see langword="false"/> which is the default.
        /// </returns>
        public virtual bool RequiresDisposal
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        ///     Releases the unmanaged resources used by the <see cref="ComposablePart"/> and 
        ///     optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">
        ///     <see langword="true"/> to release both managed and unmanaged resources; 
        ///     <see langword="false"/> to release only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing) 
        {
            _isDisposed = true;
        }

        internal void ThrowIfDisposed()
        {
            if (this._isDisposed)
            {
                throw ExceptionBuilder.CreateObjectDisposed(this);
            }
        }
    }
}
