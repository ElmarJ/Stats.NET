// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.ObjectModel;
using Microsoft.Internal;
using Microsoft.Internal.Collections;

namespace System.ComponentModel.Composition.Hosting
{
    public abstract partial class ExportProvider
    {
        /// <summary>
        ///     Returns the export with the contract name derived from the specified type parameter, 
        ///     throwing an exception if there is not exactly one matching export.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the <see cref="Export{T}"/> object to return. The contract name is also 
        ///     derived from this type parameter.
        /// </typeparam>
        /// <returns>
        ///     The <see cref="Export{T}"/> object with the contract name derived from 
        ///     <typeparamref name="T"/>.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         The returned <see cref="Export{T}"/> object is an instance of 
        ///         <see cref="Export{T, TMetadataView}"/> underneath, where 
        ///         <c>TMetadataView</c>
        ///         is <see cref="IDictionary{TKey, TValue}"/> and where <c>TKey</c> 
        ///         is <see cref="String"/> and <c>TValue</c> is <see cref="Object"/>.
        ///     </para>
        ///     <para>
        ///         The contract name is the result of calling 
        ///         <see cref="AttributedModelServices.GetContractName(Type)"/> on <typeparamref name="T"/>.
        ///     </para>
        ///     <para>
        ///         The contract name is compared using a case-sensitive, non-linguistic comparison 
        ///         using <see cref="StringComparer.Ordinal"/>.
        ///     </para>
        /// </remarks>
        /// <exception cref="ImportCardinalityMismatchException">
        ///     <para>
        ///         There are zero <see cref="Export{T}"/> objects with the contract name derived 
        ///         from <typeparamref name="T"/> in the <see cref="CompositionContainer"/>.
        ///     </para>
        ///     -or-
        ///     <para>
        ///         There are more than one <see cref="Export{T}"/> objects with the contract name 
        ///         derived from <typeparamref name="T"/> in the <see cref="CompositionContainer"/>.
        ///     </para>
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        ///     The <see cref="CompositionContainer"/> has been disposed of.
        /// </exception>
        public Export<T> GetExport<T>()
        {
            return this.GetExportCore<T>((string)null);
        }

        /// <summary>
        ///     Returns the export with the specified contract name, throwing an exception if there 
        ///     is not exactly one matching export.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the <see cref="Export{T}"/> object to return.
        /// </typeparam>
        /// <param name="contractName">
        ///     A <see cref="String"/> containing the contract name of the <see cref="Export{T}"/> 
        ///     object to return; or <see langword="null"/> or an empty string ("") to use the 
        ///     default contract name.
        /// </param>
        /// <returns>
        ///     The <see cref="Export{T}"/> object with the specified contract name.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         The returned <see cref="Export{T}"/> object is an instance of 
        ///         <see cref="Export{T, TMetadataView}"/> underneath, where 
        ///         <c>TMetadataView</c>
        ///         is <see cref="IDictionary{TKey, TValue}"/> and where <c>TKey</c> 
        ///         is <see cref="String"/> and <c>TValue</c> is <see cref="Object"/>.
        ///     </para>
        ///     <para>
        ///         The contract name is the result of calling 
        ///         <see cref="AttributedModelServices.GetContractName(Type)"/> on <typeparamref name="T"/>.
        ///     </para>
        ///     <para>
        ///         The default contract name is compared using a case-sensitive, non-linguistic 
        ///         comparison using <see cref="StringComparer.Ordinal"/>.
        ///     </para>
        /// </remarks>
        /// <exception cref="ImportCardinalityMismatchException">
        ///     <para>
        ///         There are zero <see cref="Export{T}"/> objects with the specified contract name 
        ///         in the <see cref="CompositionContainer"/>.
        ///     </para>
        ///     -or-
        ///     <para>
        ///         There are more than one <see cref="Export{T}"/> objects with the specified contract
        ///         name in the <see cref="CompositionContainer"/>.
        ///     </para>
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        ///     The <see cref="CompositionContainer"/> has been disposed of.
        /// </exception>
        public Export<T> GetExport<T>(string contractName)
        {
            return this.GetExportCore<T>(contractName);
        }

        /// <summary>
        ///     Returns the export with the contract name derived from the specified type parameter, 
        ///     throwing an exception if there is not exactly one matching export.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the <see cref="Export{T, TMetadataView}"/> object to return. The 
        ///     contract name is also derived from this type parameter.
        /// </typeparam>
        /// <typeparam name="TMetadataView">
        ///     The type of the metadata view of the <see cref="Export{T, TMetadataView}"/> object
        ///     to return.
        /// </typeparam>
        /// <returns>
        ///     The <see cref="Export{T, TMetadataView}"/> object with the contract name derived 
        ///     from <typeparamref name="T"/>.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         The contract name is the result of calling 
        ///         <see cref="AttributedModelServices.GetContractName(Type)"/> on <typeparamref name="T"/>.
        ///     </para>
        ///     <para>
        ///         The contract name is compared using a case-sensitive, non-linguistic comparison 
        ///         using <see cref="StringComparer.Ordinal"/>.
        ///     </para>
        /// </remarks>
        /// <exception cref="ImportCardinalityMismatchException">
        ///     <para>
        ///         There are zero <see cref="Export{T, TMetadataView}"/> objects with the contract 
        ///         name derived from <typeparamref name="T"/> in the 
        ///         <see cref="CompositionContainer"/>.
        ///     </para>
        ///     -or-
        ///     <para>
        ///         There are more than one <see cref="Export{T, TMetadataView}"/> objects with the 
        ///         contract name derived from <typeparamref name="T"/> in the 
        ///         <see cref="CompositionContainer"/>.
        ///     </para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     <typeparamref name="TMetadataView"/> is not a valid metadata view type.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        ///     The <see cref="CompositionContainer"/> has been disposed of.
        /// </exception>
        public Export<T, TMetadataView> GetExport<T, TMetadataView>()
        {
            return this.GetExportCore<T, TMetadataView>((string)null);
        }

        /// <summary>
        ///     Returns the export with the specified contract name, throwing an exception if there 
        ///     is not exactly one matching export.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the <see cref="Export{T, TMetadataView}"/> object to return.
        /// </typeparam>
        /// <typeparam name="TMetadataView">
        ///     The type of the metadata view of the <see cref="Export{T, TMetadataView}"/> object
        ///     to return.
        /// </typeparam>
        /// <param name="contractName">
        ///     A <see cref="String"/> containing the contract name of the 
        ///     <see cref="Export{T, TMetadataView}"/> object to return; or <see langword="null"/> 
        ///     or an empty string ("") to use the default contract name.
        /// </param>
        /// <returns>
        ///     The <see cref="Export{T, TMetadataView}"/> object with the specified contract name.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         The default contract name is the result of calling 
        ///         <see cref="AttributedModelServices.GetContractName(Type)"/> on <typeparamref name="T"/>.
        ///     </para>
        ///     <para>
        ///         The contract name is compared using a case-sensitive, non-linguistic comparison 
        ///         using <see cref="StringComparer.Ordinal"/>.
        ///     </para>
        /// </remarks>
        /// <exception cref="ImportCardinalityMismatchException">
        ///     <para>
        ///         There are zero <see cref="Export{T, TMetadataView}"/> objects with the 
        ///         specified contract name in the <see cref="CompositionContainer"/>.
        ///     </para>
        ///     -or-
        ///     <para>
        ///         There are more than one <see cref="Export{T, TMetadataView}"/> objects with the 
        ///         specified contract name in the <see cref="CompositionContainer"/>.
        ///     </para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     <typeparamref name="TMetadataView"/> is not a valid metadata view type.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        ///     The <see cref="CompositionContainer"/> has been disposed of.
        /// </exception>
        public Export<T, TMetadataView> GetExport<T, TMetadataView>(string contractName)
        {
            return this.GetExportCore<T, TMetadataView>(contractName);
        }

        /// <summary>
        ///     Returns the exports with the specified contract name.
        /// </summary>
        /// <param name="type">
        ///     The <see cref="Type"/> of the <see cref="Export"/> objects to return.
        /// </param>
        /// <param name="metadataViewType">
        ///     The <see cref="Type"/> of the metadata view of the <see cref="Export"/> objects to
        ///     return.
        /// </param>
        /// <param name="contractName">
        ///     A <see cref="String"/> containing the contract name of the 
        ///     <see cref="Export"/> object to return; or <see langword="null"/> 
        ///     or an empty string ("") to use the default contract name.
        /// </param>
        /// <returns>
        ///     An <see cref="ExportCollection"/> containing the <see cref="Export"/> objects 
        ///     with the specified contract name, if found; otherwise, an empty 
        ///     <see cref="ExportCollection"/>.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         The returned <see cref="Export"/> objects are instances of 
        ///         <see cref="Export{T, TMetadataView}"/> underneath, where <c>T</c>
        ///         is <paramref name="type"/> and <c>TMetadataView</c> is 
        ///         <paramref name="metadataViewType"/>.
        ///     </para>
        ///     <para>
        ///         The default contract name is the result of calling 
        ///         <see cref="AttributedModelServices.GetContractName(Type)"/> on <paramref name="type"/>.
        ///     </para>
        ///     <para>
        ///         The contract name is compared using a case-sensitive, non-linguistic comparison 
        ///         using <see cref="StringComparer.Ordinal"/>.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="type"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     <paramref name="metadataViewType"/> is not a valid metadata view type.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        ///     The <see cref="CompositionContainer"/> has been disposed of.
        /// </exception>
        public ExportCollection GetExports(Type type, Type metadataViewType, string contractName)
        {
            return this.GetExportsCore(type, metadataViewType, contractName);
        }

        /// <summary>
        ///     Returns the exports with the contract name derived from the specified type parameter.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the <see cref="Export{T}"/> objects to return. The contract name is also 
        ///     derived from this type parameter.
        /// </typeparam>
        /// <returns>
        ///     An <see cref="ExportCollection{T}"/> containing the <see cref="Export{T}"/> objects
        ///     with the contract name derived from <typeparamref name="T"/>, if found; otherwise,
        ///     an empty <see cref="ExportCollection{T}"/>.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         The returned <see cref="Export{T}"/> objects are instances of 
        ///         <see cref="Export{T, TMetadataView}"/> underneath, where 
        ///         <c>TMetadataView</c>
        ///         is <see cref="IDictionary{TKey, TValue}"/> and where <c>TKey</c> 
        ///         is <see cref="String"/> and <c>TValue</c> is <see cref="Object"/>.
        ///     </para>
        ///     <para>
        ///         The contract name is the result of calling 
        ///         <see cref="AttributedModelServices.GetContractName(Type)"/> on <typeparamref name="T"/>.
        ///     </para>
        ///     <para>
        ///         The contract name is compared using a case-sensitive, non-linguistic comparison 
        ///         using <see cref="StringComparer.Ordinal"/>.
        ///     </para>
        /// </remarks>
        /// <exception cref="ObjectDisposedException">
        ///     The <see cref="CompositionContainer"/> has been disposed of.
        /// </exception>
        public ExportCollection<T> GetExports<T>()
        {
            return this.GetExportsCore<T>((string)null);
        }

        /// <summary>
        ///     Returns the exports with the specified contract name.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the <see cref="Export{T}"/> objects to return.
        /// </typeparam>
        /// <param name="contractName">
        ///     A <see cref="String"/> containing the contract name of the <see cref="Export{T}"/> 
        ///     objects to return; or <see langword="null"/> or an empty string ("") to use the 
        ///     default contract name.
        /// </param>
        /// <returns>
        ///     An <see cref="ExportCollection{T}"/> containing the <see cref="Export{T}"/> objects
        ///     with the specified contract name, if found; otherwise, an empty 
        ///     <see cref="ExportCollection{T}"/>.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         The returned <see cref="Export{T}"/> objects are instances of 
        ///         <see cref="Export{T, TMetadataView}"/> underneath, where 
        ///         <c>TMetadataView</c>
        ///         is <see cref="IDictionary{TKey, TValue}"/> and where <c>TKey</c> 
        ///         is <see cref="String"/> and <c>TValue</c> is <see cref="Object"/>.
        ///     </para>
        ///     <para>
        ///         The default contract name is the result of calling 
        ///         <see cref="AttributedModelServices.GetContractName(Type)"/> on <typeparamref name="T"/>.
        ///     </para>
        ///     <para>
        ///         The contract name is compared using a case-sensitive, non-linguistic comparison 
        ///         using <see cref="StringComparer.Ordinal"/>.
        ///     </para>
        /// </remarks>
        /// <exception cref="ObjectDisposedException">
        ///     The <see cref="CompositionContainer"/> has been disposed of.
        /// </exception>
        public ExportCollection<T> GetExports<T>(string contractName)
        {
            return this.GetExportsCore<T>(contractName);
        }

        /// <summary>
        ///     Returns the exports with the contract name derived from the specified type parameter.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the <see cref="Export{T, TMetadataView}"/> objects to return. The 
        ///     contract name is also derived from this type parameter.
        /// </typeparam>
        /// <typeparam name="TMetadataView">
        ///     The type of the metadata view of the <see cref="Export{T, TMetadataView}"/> objects
        ///     to return.
        /// </typeparam>
        /// <returns>
        ///     An <see cref="ExportCollection{T, TMetadataView}"/> containing the 
        ///     <see cref="Export{T, TMetadataView}"/> objects with the contract name derived from 
        ///     <typeparamref name="T"/>, if found; otherwise, an empty 
        ///     <see cref="ExportCollection{T, TMetadataView}"/>.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         The contract name is the result of calling 
        ///         <see cref="AttributedModelServices.GetContractName(Type)"/> on <typeparamref name="T"/>.
        ///     </para>
        ///     <para>
        ///         The contract name is compared using a case-sensitive, non-linguistic comparison 
        ///         using <see cref="StringComparer.Ordinal"/>.
        ///     </para>
        /// </remarks>
        /// <exception cref="InvalidOperationException">
        ///     <typeparamref name="TMetadataView"/> is not a valid metadata view type.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        ///     The <see cref="CompositionContainer"/> has been disposed of.
        /// </exception>
        public ExportCollection<T, TMetadataView> GetExports<T, TMetadataView>()
        {
            return this.GetExportsCore<T, TMetadataView>((string)null);
        }

        /// <summary>
        ///     Returns the exports with the specified contract name.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the <see cref="Export{T, TMetadataView}"/> objects to return. The 
        ///     contract name is also derived from this type parameter.
        /// </typeparam>
        /// <typeparam name="TMetadataView">
        ///     The type of the metadata view of the <see cref="Export{T, TMetadataView}"/> objects
        ///     to return.
        /// </typeparam>
        /// <param name="contractName">
        ///     A <see cref="String"/> containing the contract name of the 
        ///     <see cref="Export{T, TMetadataView}"/> objects to return; or <see langword="null"/> 
        ///     or an empty string ("") to use the default contract name.
        /// </param>
        /// <returns>
        ///     An <see cref="ExportCollection{T, TMetadataView}"/> containing the 
        ///     <see cref="Export{T, TMetadataView}"/> objects with the specified contract name if 
        ///     found; otherwise, an empty <see cref="ExportCollection{T, TMetadataView}"/>.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         The default contract name is the result of calling 
        ///         <see cref="AttributedModelServices.GetContractName(Type)"/> on <typeparamref name="T"/>.
        ///     </para>
        ///     <para>
        ///         The contract name is compared using a case-sensitive, non-linguistic comparison 
        ///         using <see cref="StringComparer.Ordinal"/>.
        ///     </para>
        /// </remarks>
        /// <exception cref="InvalidOperationException">
        ///     <typeparamref name="TMetadataView"/> is not a valid metadata view type.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        ///     The <see cref="CompositionContainer"/> has been disposed of.
        /// </exception>
        public ExportCollection<T, TMetadataView> GetExports<T, TMetadataView>(string contractName)
        {
            return this.GetExportsCore<T, TMetadataView>(contractName);
        }

        /// <summary>
        ///     Returns the exported object with the contract name derived from the specified type 
        ///     parameter, throwing an exception if there is not exactly one matching exported object.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the exported object to return. The contract name is also 
        ///     derived from this type parameter.
        /// </typeparam>
        /// <returns>
        ///     The exported <see cref="Object"/> with the contract name derived from 
        ///     <typeparamref name="T"/>.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         The contract name is the result of calling 
        ///         <see cref="AttributedModelServices.GetContractName(Type)"/> on <typeparamref name="T"/>.
        ///     </para>
        ///     <para>
        ///         The contract name is compared using a case-sensitive, non-linguistic comparison 
        ///         using <see cref="StringComparer.Ordinal"/>.
        ///     </para>
        /// </remarks>
        /// <exception cref="CompositionContractMismatchException">
        ///     The underlying exported object cannot be cast to <typeparamref name="T"/>.
        /// </exception>
        /// <exception cref="ImportCardinalityMismatchException">
        ///     <para>
        ///         There are zero exported objects with the contract name derived from 
        ///         <typeparamref name="T"/> in the <see cref="CompositionContainer"/>.
        ///     </para>
        ///     -or-
        ///     <para>
        ///         There are more than one exported objects with the contract name derived from
        ///         <typeparamref name="T"/> in the <see cref="CompositionContainer"/>.
        ///     </para>
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        ///     The <see cref="CompositionContainer"/> has been disposed of.
        /// </exception>
        /// <exception cref="CompositionException">
        ///     An error occurred during composition. <see cref="CompositionException.Errors"/> will 
        ///     contain a collection of errors that occurred.
        /// </exception>
        public T GetExportedObject<T>()
        {
            return this.GetExportedObjectCore<T>((string)null, ImportCardinality.ExactlyOne);
        }

        /// <summary>
        ///     Returns the exported object with the specified contract name, throwing an exception 
        ///     if there is not exactly one matching exported object.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the exported object to return.
        /// </typeparam>
        /// <param name="contractName">
        ///     A <see cref="String"/> containing the contract name of the exported object to return,
        ///     or <see langword="null"/> or an empty string ("") to use the default contract name.
        /// </param>
        /// <returns>
        ///     The exported <see cref="Object"/> with the specified contract name.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         The default contract name is the result of calling 
        ///         <see cref="AttributedModelServices.GetContractName(Type)"/> on <typeparamref name="T"/>.
        ///     </para>
        ///     <para>
        ///         The contract name is compared using a case-sensitive, non-linguistic comparison 
        ///         using <see cref="StringComparer.Ordinal"/>.
        ///     </para>
        /// </remarks>
        /// <exception cref="CompositionContractMismatchException">
        ///     The underlying exported object cannot be cast to <typeparamref name="T"/>.
        /// </exception>
        /// <exception cref="ImportCardinalityMismatchException">
        ///     <para>
        ///         There are zero exported objects with the specified contract name in the 
        ///         <see cref="CompositionContainer"/>.
        ///     </para>
        ///     -or-
        ///     <para>
        ///         There are more than one exported objects with the specified contract name in the
        ///         <see cref="CompositionContainer"/>.
        ///     </para>
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        ///     The <see cref="CompositionContainer"/> has been disposed of.
        /// </exception>
        /// <exception cref="CompositionException">
        ///     An error occurred during composition. <see cref="CompositionException.Errors"/> will 
        ///     contain a collection of errors that occurred.
        /// </exception>
        public T GetExportedObject<T>(string contractName)
        {
            return this.GetExportedObjectCore<T>(contractName, ImportCardinality.ExactlyOne);
        }

        /// <summary>
        ///     Returns the exported object with the contract name derived from the specified type 
        ///     parameter, throwing an exception if there is more than one matching exported object.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the exported object to return. The contract name is also 
        ///     derived from this type parameter.
        /// </typeparam>
        /// <returns>
        ///     The exported <see cref="Object"/> with the contract name derived from 
        ///     <typeparamref name="T"/>, if found; otherwise, the default value for
        ///     <typeparamref name="T"/>.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         If the exported object is not found, then this method returns the appropriate 
        ///         default value for <typeparamref name="T"/>; for example, 0 (zero) for integer 
        ///         types, <see langword="false"/> for Boolean types, and <see langword="null"/> 
        ///         for reference types.
        ///     </para>
        ///     <para>
        ///         The contract name is the result of calling 
        ///         <see cref="AttributedModelServices.GetContractName(Type)"/> on <typeparamref name="T"/>.
        ///     </para>
        ///     <para>
        ///         The contract name is compared using a case-sensitive, non-linguistic comparison 
        ///         using <see cref="StringComparer.Ordinal"/>.
        ///     </para>
        /// </remarks>
        /// <exception cref="CompositionContractMismatchException">
        ///     The underlying exported object cannot be cast to <typeparamref name="T"/>.
        /// </exception>
        /// <exception cref="ImportCardinalityMismatchException">
        ///     <para>
        ///         There are more than one exported objects with the contract name derived from
        ///         <typeparamref name="T"/> in the <see cref="CompositionContainer"/>.
        ///     </para>
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        ///     The <see cref="CompositionContainer"/> has been disposed of.
        /// </exception>
        /// <exception cref="CompositionException">
        ///     An error occurred during composition. <see cref="CompositionException.Errors"/> will 
        ///     contain a collection of errors that occurred.
        /// </exception>
        public T GetExportedObjectOrDefault<T>()
        {
            return this.GetExportedObjectCore<T>((string)null, ImportCardinality.ZeroOrOne);
        }

        /// <summary>
        ///     Returns the exported object with the specified contract name, throwing an exception 
        ///     if there is more than one matching exported object.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the exported object to return.
        /// </typeparam>
        /// <param name="contractName">
        ///     A <see cref="String"/> containing the contract name of the exported object to return,
        ///     or <see langword="null"/> or an empty string ("") to use the default contract name.
        /// </param>
        /// <returns>
        ///     The exported <see cref="Object"/> with the specified contract name, if found; 
        ///     otherwise, the default value for <typeparamref name="T"/>.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         If the exported object is not found, then this method returns the appropriate 
        ///         default value for <typeparamref name="T"/>; for example, 0 (zero) for integer 
        ///         types, <see langword="false"/> for Boolean types, and <see langword="null"/> 
        ///         for reference types.
        ///     </para>
        ///     <para>
        ///         The default contract name is the result of calling 
        ///         <see cref="AttributedModelServices.GetContractName(Type)"/> on <typeparamref name="T"/>.
        ///     </para>
        ///     <para>
        ///         The contract name is compared using a case-sensitive, non-linguistic comparison 
        ///         using <see cref="StringComparer.Ordinal"/>.
        ///     </para>
        /// </remarks>
        /// <exception cref="CompositionContractMismatchException">
        ///     The underlying exported object cannot be cast to <typeparamref name="T"/>.
        /// </exception>
        /// <exception cref="ImportCardinalityMismatchException">
        ///     There are more than one exported objects with the specified contract name in the
        ///     <see cref="CompositionContainer"/>.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        ///     The <see cref="CompositionContainer"/> has been disposed of.
        /// </exception>
        /// <exception cref="CompositionException">
        ///     An error occurred during composition. <see cref="CompositionException.Errors"/> will 
        ///     contain a collection of errors that occurred.
        /// </exception>
        public T GetExportedObjectOrDefault<T>(string contractName)
        {
            return this.GetExportedObjectCore<T>(contractName, ImportCardinality.ZeroOrOne);
        }

        /// <summary>
        ///     Returns the exported objects with the contract name derived from the specified type 
        ///     parameter.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the exported object to return. The contract name is also 
        ///     derived from this type parameter.
        /// </typeparam>
        /// <returns>
        ///     An <see cref="Collection{T}"/> containing the exported objects with the contract name 
        ///     derived from the specified type parameter, if found; otherwise, an empty 
        ///     <see cref="Collection{T}"/>.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         The contract name is the result of calling 
        ///         <see cref="AttributedModelServices.GetContractName(Type)"/> on <typeparamref name="T"/>.
        ///     </para>
        ///     <para>
        ///         The contract name is compared using a case-sensitive, non-linguistic comparison 
        ///         using <see cref="StringComparer.Ordinal"/>.
        ///     </para>
        /// </remarks>
        /// <exception cref="CompositionContractMismatchException">
        ///     One or more of the underlying exported objects cannot be cast to 
        ///     <typeparamref name="T"/>.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        ///     The <see cref="CompositionContainer"/> has been disposed of.
        /// </exception>
        /// <exception cref="CompositionException">
        ///     An error occurred during composition. <see cref="CompositionException.Errors"/> will 
        ///     contain a collection of errors that occurred.
        /// </exception>
        public Collection<T> GetExportedObjects<T>()
        {
            return this.GetExportedObjectsCore<T>((string)null);
        }

        /// <summary>
        ///     Returns the exported objects with the specified contract name.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the exported object to return.
        /// </typeparam>
        /// <param name="contractName">
        ///     A <see cref="String"/> containing the contract name of the exported objects to 
        ///     return; or <see langword="null"/> or an empty string ("") to use the default 
        ///     contract name.
        /// </param>
        /// <returns>
        ///     An <see cref="Collection{T}"/> containing the exported objects with the specified 
        ///     contract name, if found; otherwise, an empty <see cref="Collection{T}"/>.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         The default contract name is the result of calling 
        ///         <see cref="AttributedModelServices.GetContractName(Type)"/> on <typeparamref name="T"/>.
        ///     </para>
        ///     <para>
        ///         The contract name is compared using a case-sensitive, non-linguistic comparison 
        ///         using <see cref="StringComparer.Ordinal"/>.
        ///     </para>
        /// </remarks>
        /// <exception cref="CompositionContractMismatchException">
        ///     One or more of the underlying exported objects cannot be cast to 
        ///     <typeparamref name="T"/>.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        ///     The <see cref="CompositionContainer"/> has been disposed of.
        /// </exception>
        /// <exception cref="CompositionException">
        ///     An error occurred during composition. <see cref="CompositionException.Errors"/> will 
        ///     contain a collection of errors that occurred.
        /// </exception>
        public Collection<T> GetExportedObjects<T>(string contractName)
        {
            return this.GetExportedObjectsCore<T>(contractName);
        }

        private Export<T> GetExportCore<T>(string contractName)
        {
            Export<T> export = this.GetExportCore<Export<T>>(typeof(T), (Type)null, contractName, ImportCardinality.ExactlyOne);

            return export;
        }

        private Export<T, TMetadataView> GetExportCore<T, TMetadataView>(string contractName)
        {
            Export<T, TMetadataView> export = this.GetExportCore<Export<T, TMetadataView>>(typeof(T), typeof(TMetadataView), contractName, ImportCardinality.ExactlyOne);

            return export;
        }

        private ExportCollection GetExportsCore(Type type, Type metadataViewType, string contractName)
        {
            IEnumerable<Export> exports = this.GetExportsCore<Export>(type, metadataViewType, contractName, ImportCardinality.ZeroOrMore);

            return new ExportCollection(exports);
        }

        private ExportCollection<T> GetExportsCore<T>(string contractName)
        {
            IEnumerable<Export<T>> exports = this.GetExportsCore<Export<T>>(typeof(T), (Type)null, contractName, ImportCardinality.ZeroOrMore);

            return new ExportCollection<T>(exports);
        }

        private ExportCollection<T, TMetadataView> GetExportsCore<T, TMetadataView>(string contractName)
        {
            IEnumerable<Export<T, TMetadataView>> exports = this.GetExportsCore<Export<T, TMetadataView>>(typeof(T), typeof(TMetadataView), contractName, ImportCardinality.ZeroOrMore);

            return new ExportCollection<T, TMetadataView>(exports);
        }

        private Collection<T> GetExportedObjectsCore<T>(string contractName)
        {
            IEnumerable<Export<T>> exports = this.GetExportsCore<Export<T>>(typeof(T), (Type)null, contractName, ImportCardinality.ZeroOrMore);

            return exports.Select(export => export.GetExportedObject()).ToCollection();
        }

        private T GetExportedObjectCore<T>(string contractName, ImportCardinality cardinality)
        {
            Export<T> export = this.GetExportCore<Export<T>>(typeof(T), (Type)null, contractName, cardinality);
            if (export != null)
            {
                return export.GetExportedObject();
            }

            return default(T);
        }

        private TExport GetExportCore<TExport>(Type type, Type metadataViewType, string contractName, ImportCardinality cardinality) where TExport : Export
        {
            Assumes.IsTrue(cardinality.IsAtMostOne());

            IEnumerable<TExport> exports = this.GetExportsCore<TExport>(type, metadataViewType, contractName, cardinality);

            // There should only ever be zero or 
            // one, so this should never throw
            return exports.SingleOrDefault();
        }

        private IEnumerable<TExport> GetExportsCore<TExport>(Type type, Type metadataViewType, string contractName, ImportCardinality cardinality) where TExport : Export
        {
            // Only 'type' cannot be null - the other parameters have sensible defaults.
            Requires.NotNull(type, "type");

            if (string.IsNullOrEmpty(contractName))
            {
                contractName = AttributedModelServices.GetContractName(type);
            }

            if (metadataViewType == null)
            {
                metadataViewType = ExportServices.DefaultMetadataViewType;
            }

            if (!MetadataViewProvider.IsViewTypeValid(metadataViewType))
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, Strings.InvalidMetadataView, metadataViewType.Name));
            }

            ImportDefinition importDefinition = BuildImportDefinition(type, metadataViewType, contractName, cardinality);

            IEnumerable<Export> exports = this.GetExports(importDefinition);

            Func<Export, object> exportFunc = ExportServices.CreateStronglyTypedExportFactory(type, metadataViewType);
            return exports.Select(export =>
            {
                return (TExport)exportFunc(export);
            });
        }

        private static ImportDefinition BuildImportDefinition(Type type, Type metadataViewType, string contractName, ImportCardinality cardinality)
        {
            Assumes.NotNull(type, metadataViewType, contractName);

            IEnumerable<string> requiredMetadata = CompositionServices.GetRequiredMetadata(metadataViewType);

            string requiredTypeIdentity = null;
            if (type != typeof(object))
            {
                requiredTypeIdentity = AttributedModelServices.GetTypeIdentity(type);
            }

            return new ContractBasedImportDefinition(contractName, requiredTypeIdentity, requiredMetadata, cardinality, false, true, CreationPolicy.Any);
        }
    }
}
