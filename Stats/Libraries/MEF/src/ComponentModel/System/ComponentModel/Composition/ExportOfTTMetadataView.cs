// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using Microsoft.Internal;

namespace System.ComponentModel.Composition
{
    /// <summary>
    ///     Represents an export. That is, a type that is made up of a delay-created exported object, 
    ///     metadata that describes that object and a strongly-typed view over that metadata.
    /// </summary>
    public class Export<T, TMetadataView> : Export<T>
    {
        private bool _metadataViewRetrieved;
        private TMetadataView _metadataView;
        
        /// <summary>
        ///     Initializes a new instance of the <see cref="Export{T, TMetadataView}"/> class.
        /// </summary>
        /// <remarks>
        ///     <note type="inheritinfo">
        ///         Derived types calling this constructor must override <see cref="ExportDefinition"/> 
        ///         and <see cref="Export.GetExportedObjectCore"/>.
        ///     </note>
        /// </remarks>
        protected Export()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Export{T}"/> class 
        ///     with the specified metadata, exported object getter and using a 
        ///     contract name derived from <typeparamref name="T"/>.
        /// </summary>
        /// <param name="metadata">
        ///     An <see cref="IDictionary{TKey, TValue}"/> containing the metadata of the 
        ///     <see cref="Export{T, TMetadataView}"/>; or <see langword="null"/> to set the 
        ///     <see cref="Export.Metadata"/> property to an empty, read-only 
        ///     <see cref="IDictionary{TKey, TValue}"/>.
        /// </param>
        /// <param name="exportedObjectGetter">
        ///     A <see cref="Func{T}"/> that is called to create the exported object of the 
        ///     <see cref="Export{T}"/>. This allows the creation of the object to be delayed 
        ///     until <see cref="Export{T}.GetExportedObject"/> is called.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="exportedObjectGetter"/> is <see langword="null"/>.
        /// </exception>
        /// <remarks>
        ///     <para>
        ///         The <see cref="ExportDefinition.ContractName"/> property of <see cref="Export.Definition"/> 
        ///         is the result of calling <see cref="AttributedModelServices.GetContractName(Type)"/> on 
        ///         <typeparamref name="T"/>.
        ///     </para>
        /// </remarks>
        public Export(IDictionary<string, object> metadata, Func<T> exportedObjectGetter)
            : base(GetContractName(), metadata, exportedObjectGetter)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Export{T, TMetadataView}"/> class 
        ///     with the specified contract name, metadata and exported object getter.
        /// </summary>
        /// <param name="contractName">
        ///     A <see cref="String"/> containing the contract name of the 
        ///     <see cref="Export{T, TMetadataView}"/>.
        /// </param>
        /// <param name="metadata">
        ///     An <see cref="IDictionary{TKey, TValue}"/> containing the metadata of the 
        ///     <see cref="Export{T, TMetadataView}"/>; or <see langword="null"/> to set the 
        ///     <see cref="Export.Metadata"/> property to an empty, read-only 
        ///     <see cref="IDictionary{TKey, TValue}"/>.
        /// </param>
        /// <param name="exportedObjectGetter">
        ///     A <see cref="Func{T}"/> that is called to create the exported object of the 
        ///     <see cref="Export{T, TMetadataView}"/>. This allows the creation of the object to be delayed 
        ///     until <see cref="Export{T}.GetExportedObject"/> is called.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="contractName"/> is <see langword="null"/>.
        ///     <para>
        ///         -or-
        ///     </para>
        ///     <paramref name="exportedObjectGetter"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <paramref name="contractName"/> is an empty string ("").
        /// </exception>
        public Export(string contractName, IDictionary<string, object> metadata, Func<T> exportedObjectGetter)
            : base(contractName, metadata, exportedObjectGetter)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Export{T, TMetadataView}"/> class 
        ///     with the specified export definition and exported object getter.
        /// </summary>
        /// <param name="definition">
        ///     An <see cref="ExportDefinition"/> that describes the contract that the 
        ///     <see cref="Export{T, TMetadataView}"/> satisfies.
        /// </param>
        /// <param name="exportedObjectGetter">
        ///     A <see cref="Func{T}"/> that is called to create the exported object of the 
        ///     <see cref="Export{T, TMetadataView}"/>. This allows the creation of the object to be delayed 
        ///     until <see cref="Export{T}.GetExportedObject"/> is called.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="definition"/> is <see langword="null"/>.
        ///     <para>
        ///         -or-
        ///     </para>
        ///     <paramref name="exportedObjectGetter"/> is <see langword="null"/>.
        /// </exception>
        public Export(ExportDefinition definition, Func<T> exportedObjectGetter)
            : base(definition, exportedObjectGetter)
        {
        }

        /// <summary>
        ///     Gets the metadata view for the export.
        /// </summary>
        /// <value>
        ///     A <typeparamref name="TMetadataView"/> representing the strongly-typed metadata view 
        ///     for the <see cref="Export{T, TMetadataView}"/>.
        /// </value>
        /// <exception cref="CompositionContractMismatchException">
        ///     The <see cref="Export.Metadata"/> property cannot be converted to <typeparamref name="TMetadataView"/>.
        /// </exception>
        /// <exception cref="NotImplementedException">
        ///     The <see cref="Export.Definition"/> property was not overridden by a derived class.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     <typeparamref name="TMetadataView"/> is not a valid metadata view type.
        /// </exception>
        /// <remarks>
        ///     <para>
        ///         This property represents a strong-typed view over the <see cref="Export.Metadata"/> property.
        ///     </para>
        /// </remarks>
        public virtual TMetadataView MetadataView
        {
            get 
            {
                if (!_metadataViewRetrieved)
                {
                    _metadataView = MetadataViewProvider.GetMetadataView<TMetadataView>(Metadata);
                    _metadataViewRetrieved = true;
                }

                return _metadataView;
            }
        }
    }
}
