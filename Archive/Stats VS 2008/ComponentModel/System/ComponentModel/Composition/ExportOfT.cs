// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Microsoft.Internal;

namespace System.ComponentModel.Composition
{
    /// <summary>
    ///     Represents an export. That is, a type that is made up of a delay-created exported object
    ///     and metadata that describes that object.
    /// </summary>
    public class Export<T> : Export
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Export{T}"/> class.
        /// </summary>
        /// <remarks>
        ///     <note type="inheritinfo">
        ///         Derived types calling this constructor must override <see cref="Export.Definition"/>
        ///         and <see cref="Export.GetExportedObjectCore"/>.
        ///     </note>
        /// </remarks>
        protected Export()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Export{T}"/> class 
        ///     with the specified exported object getter and using a contract 
        ///     name derived from <typeparamref name="T"/>.
        /// </summary>
        /// <param name="exportedObjectGetter">
        ///     A <see cref="Func{T}"/> that is called to create the exported object of the 
        ///     <see cref="Export{T}"/>. This allows the creation of the object to be delayed 
        ///     until  <see cref="GetExportedObject"/> is called.
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
        public Export(Func<T> exportedObjectGetter)
            : base(GetContractName(), GetExportedObjectGetter(exportedObjectGetter))
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Export{T}"/> class 
        ///     with the specified contract name and exported object getter.
        /// </summary>
        /// <param name="contractName">
        ///     A <see cref="String"/> containing the contract name of the 
        ///     <see cref="Export{T}"/>.
        /// </param>
        /// <param name="exportedObjectGetter">
        ///     A <see cref="Func{T}"/> that is called to create the exported object of the 
        ///     <see cref="Export{T}"/>. This allows the creation of the object to be delayed 
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
        public Export(string contractName, Func<T> exportedObjectGetter)
            : base(contractName, GetExportedObjectGetter(exportedObjectGetter))
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Export{T}"/> class 
        ///     with the specified contract name, metadata and exported object getter.
        /// </summary>
        /// <param name="contractName">
        ///     A <see cref="String"/> containing the contract name of the 
        ///     <see cref="Export{T}"/>.
        /// </param>
        /// <param name="metadata">
        ///     An <see cref="IDictionary{TKey, TValue}"/> containing the metadata of the 
        ///     <see cref="Export{T}"/>; or <see langword="null"/> to set the 
        ///     <see cref="Export.Metadata"/> property to an empty, read-only 
        ///     <see cref="IDictionary{TKey, TValue}"/>.
        /// </param>
        /// <param name="exportedObjectGetter">
        ///     A <see cref="Func{T}"/> that is called to create the exported object of the 
        ///     <see cref="Export{T}"/>. This allows the creation of the object to be delayed 
        ///     until  <see cref="GetExportedObject"/> is called.
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
            : base(contractName, metadata, GetExportedObjectGetter(exportedObjectGetter))
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Export{T}"/> class 
        ///     with the specified export definition and exported object getter.
        /// </summary>
        /// <param name="definition">
        ///     An <see cref="ExportDefinition"/> that describes the contract that the 
        ///     <see cref="Export{T}"/> satisfies.
        /// </param>
        /// <param name="exportedObjectGetter">
        ///     A <see cref="Func{T}"/> that is called to create the exported object of the 
        ///     <see cref="Export{T}"/>. This allows the creation of the object to be delayed 
        ///     until  <see cref="GetExportedObject"/> is called.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="definition"/> is <see langword="null"/>.
        ///     <para>
        ///         -or-
        ///     </para>
        ///     <paramref name="exportedObjectGetter"/> is <see langword="null"/>.
        /// </exception>
        public Export(ExportDefinition definition, Func<T> exportedObjectGetter)
            : base(definition, GetExportedObjectGetter(exportedObjectGetter))
        {
        }

        /// <summary>
        ///     Returns the exported object of the export.
        /// </summary>
        /// <returns>
        ///     A <typeparamref name="T"/> representing the exported object of the 
        ///     <see cref="Export{T}"/>.
        /// </returns>
        /// <exception cref="CompositionException">
        ///     An error occurred during composition. <see cref="CompositionException.Errors"/> will
        ///     contain a collection of errors that occurred.
        /// </exception>
        /// <exception cref="CompositionContractMismatchException">
        ///     The underlying exported object cannot be cast to <typeparamref name="T"/>.
        /// </exception>
        /// <exception cref="NotImplementedException">
        ///     The <see cref="Export.GetExportedObjectCore"/> method was not overridden by a derived class.
        /// </exception>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public new T GetExportedObject()
        {
            return (T)base.GetExportedObject();
        }

        internal override void CheckExportedObject(object exportedObject)
        {
            if (exportedObject == null || exportedObject is T)
                return;

            throw new CompositionContractMismatchException(string.Format(CultureInfo.CurrentCulture,
                Strings.ContractMismatch_ExportedObjectCannotBeCastToT,
                exportedObject.GetType(),
                typeof(T)));
        }

        private static Func<object> GetExportedObjectGetter(Func<T> exportedObjectGetter)
        {
            Requires.NotNull(exportedObjectGetter, "exportedObjectGetter");

            return () => exportedObjectGetter();
        }

        internal static string GetContractName()
        {
            return AttributedModelServices.GetContractName(typeof(T));
        }
    }
}
