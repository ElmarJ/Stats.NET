// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Internal;

namespace System.ComponentModel.Composition.Primitives
{
    /// <summary>
    ///     Represents an export. That is, a type that is made up of a delay-created exported object 
    ///     and metadata that describes that object.
    /// </summary>
    public class Export
    {
        private readonly ExportDefinition _definition;
        private readonly Func<object> _exportedObjectGetter;
        private object _exportedObject;
        
        /// <summary>
        ///     Initializes a new instance of the <see cref="Export"/> class.
        /// </summary>
        /// <remarks>
        ///     <note type="inheritinfo">
        ///         Derived types calling this constructor must override <see cref="Definition"/>
        ///         and <see cref="GetExportedObjectCore"/>.
        ///     </note>
        /// </remarks>
        protected Export()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Export"/> class 
        ///     with the specified contract name and exported object getter.
        /// </summary>
        /// <param name="contractName">
        ///     A <see cref="String"/> containing the contract name of the 
        ///     <see cref="Export"/>.
        /// </param>
        /// <param name="exportedObjectGetter">
        ///     A <see cref="Func{T}"/> that is called to create the exported object of the 
        ///     <see cref="Export"/>. This allows the creation of the object to be delayed 
        ///     until <see cref="GetExportedObject"/> is called.
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
        public Export(string contractName, Func<object> exportedObjectGetter)
            : this(new ExportDefinition(contractName, (IDictionary<string, object>)null), exportedObjectGetter)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Export"/> class 
        ///     with the specified contract name, metadata and exported object getter.
        /// </summary>
        /// <param name="contractName">
        ///     A <see cref="String"/> containing the contract name of the 
        ///     <see cref="Export"/>.
        /// </param>
        /// <param name="metadata">
        ///     An <see cref="IDictionary{TKey, TValue}"/> containing the metadata of the 
        ///     <see cref="Export"/>; or <see langword="null"/> to set the 
        ///     <see cref="Metadata"/> property to an empty, read-only 
        ///     <see cref="IDictionary{TKey, TValue}"/>.
        /// </param>
        /// <param name="exportedObjectGetter">
        ///     A <see cref="Func{T}"/> that is called to create the exported object of the 
        ///     <see cref="Export"/>. This allows the creation of the object to be delayed 
        ///     until <see cref="GetExportedObject"/> is called.
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
        public Export(string contractName, IDictionary<string, object> metadata, Func<object> exportedObjectGetter) 
            : this(new ExportDefinition(contractName, metadata), exportedObjectGetter)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Export"/> class 
        ///     with the specified export definition and exported object getter.
        /// </summary>
        /// <param name="definition">
        ///     An <see cref="ExportDefinition"/> that describes the contract that the 
        ///     <see cref="Export"/> satisfies.
        /// </param>
        /// <param name="exportedObjectGetter">
        ///     A <see cref="Func{T}"/> that is called to create the exported object of the 
        ///     <see cref="Export"/>. This allows the creation of the object to be delayed 
        ///     until  <see cref="GetExportedObject"/> is called.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="definition"/> is <see langword="null"/>.
        ///     <para>
        ///         -or-
        ///     </para>
        ///     <paramref name="exportedObjectGetter"/> is <see langword="null"/>.
        /// </exception>
        public Export(ExportDefinition definition, Func<object> exportedObjectGetter)
        {
            Requires.NotNull(definition, "definition");
            Requires.NotNull(exportedObjectGetter, "exportedObjectGetter");

            this._definition = definition;
            this._exportedObjectGetter = exportedObjectGetter;
        }

        /// <summary>
        ///     Gets the definition that describes the contract that the export satisfies.
        /// </summary>
        /// <value>
        ///     An <see cref="ExportDefinition"/> that describes the contract that 
        ///     the <see cref="Export"/> satisfies.
        /// </value>
        /// <exception cref="NotImplementedException">
        ///     This property was not overridden by a derived class.
        /// </exception>
        /// <remarks>
        ///     <note type="inheritinfo">
        ///         Overriders of this property should never return
        ///         <see langword="null"/>.
        ///     </note>
        /// </remarks>
        public virtual ExportDefinition Definition
        {
            get 
            {
                if (_definition != null)
                {
                    return _definition;
                }

                throw ExceptionBuilder.CreateNotOverriddenByDerived("Definition");
            }
        }

        /// <summary>
        ///     Gets the metadata of the export.
        /// </summary>
        /// <value>
        ///     An <see cref="IDictionary{TKey, TValue}"/> containing the metadata of the 
        ///     <see cref="Export"/>.
        /// </value>
        /// <exception cref="NotImplementedException">
        ///     The <see cref="Definition"/> property was not overridden by a derived class.
        /// </exception>
        /// <remarks>
        ///     <para>
        ///         This property returns the value of <see cref="ExportDefinition.Metadata"/>
        ///         of the <see cref="Definition"/> property.
        ///     </para>
        /// </remarks>
        public IDictionary<string, object> Metadata
        {
            get { return Definition.Metadata; }
        }

        /// <summary>
        ///     Returns the exported object of the export.
        /// </summary>
        /// <returns>
        ///     The exported <see cref="Object"/> of the <see cref="Export"/>.
        /// </returns>
        /// <exception cref="CompositionException">
        ///     An error occurred during composition. <see cref="CompositionException.Errors"/> will 
        ///     contain a collection of errors that occurred.
        /// </exception>
        /// <exception cref="CompositionContractMismatchException">
        ///     The current instance is an instance of <see cref="Export{T}"/> and the underlying 
        ///     exported object cannot be cast to <c>T</c>.
        /// </exception>
        /// <exception cref="NotImplementedException">
        ///     The <see cref="GetExportedObjectCore"/> method was not overridden by a derived class.
        /// </exception>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public object GetExportedObject()
        {
            object exportedObject = this.GetExportedObjectCore();

            CheckExportedObject(exportedObject);

            return exportedObject;
        }

        /// <summary>
        ///     Returns the exported object of the export.
        /// </summary>
        /// <returns>
        ///     The exported <see cref="Object"/> of the <see cref="Export"/>.
        /// </returns>
        /// <exception cref="CompositionException">
        ///     An error occurred during composition. <see cref="CompositionException.Errors"/> will 
        ///     contain a collection of errors that occurred.
        /// </exception>
        /// <exception cref="CompositionContractMismatchException">
        ///     The current instance is an instance of <see cref="Export{T}"/> and the underlying 
        ///     exported object cannot be cast to <c>T</c>.
        /// </exception>
        /// <exception cref="NotImplementedException">
        ///     The method was not overridden by a derived class.
        /// </exception>
        /// <remarks>
        ///     <note type="inheritinfo">
        ///         Overriders of this method should never return
        ///         <see langword="null"/>.
        ///     </note>
        /// </remarks>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        protected virtual object GetExportedObjectCore()
        {
            if (_exportedObjectGetter != null)
            {
                if (_exportedObject == null)
                {
                    _exportedObject = _exportedObjectGetter();
                }

                return _exportedObject;
            }

            throw ExceptionBuilder.CreateNotOverriddenByDerived("GetExportedObjectCore");
        }

        internal virtual void CheckExportedObject(object exportedObject)
        {
        }
    }   
}
