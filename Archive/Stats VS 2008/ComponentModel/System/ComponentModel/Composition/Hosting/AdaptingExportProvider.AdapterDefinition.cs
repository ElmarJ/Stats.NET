// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Diagnostics.CodeAnalysis;
using System.ComponentModel.Composition.Primitives;
using Microsoft.Internal;
using System.ComponentModel.Composition.ReflectionModel;

namespace System.ComponentModel.Composition.Hosting
{
    partial class AdaptingExportProvider
    {
        // Represents an adapter method that adapts from one contract to another
        private class AdapterDefinition
        {
            private Func<Export, Export> _adaptMethod;
            private readonly string _fromContractName;
            private readonly string _toContractName;
            private readonly Export _export;

            public AdapterDefinition(Export export)
            {
                Assumes.NotNull(export);

                this._export = export;
                this._fromContractName = this.GetContractName(CompositionConstants.AdapterFromContractMetadataName);
                this._toContractName = this.GetContractName(CompositionConstants.AdapterToContractMetadataName);
            }

            public Export Export
            {
                get { return _export; }
            }

            public string FromContractName
            {
                get { return _fromContractName; }                
            }

            public string ToContractName
            {
                get { return _toContractName; }                
            }

            [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
            public Export Adapt(Export export)
            {
                Assumes.NotNull(_adaptMethod, export);

                Export adaptedExport = null;

                try
                {
                    adaptedExport = this._adaptMethod(export);
                }
                catch (Exception exception)
                {   // Adapter threw an exception. Avoid letting this 
                    // leak out as a 'raw' unhandled exception, instead,
                    // we'll add some context and rethrow.

                    ICompositionElement element = Export.ToElement();
                    throw new CompositionException(CompositionError.Create(
                            CompositionErrorId.Adapter_ExceptionDuringAdapt,
                            element,
                            exception,
                            Strings.Adapter_ExceptionDuringAdapt,
                            element.DisplayName,
                            this.ToContractName,
                            this.FromContractName));
                }

                CheckAdaptation(adaptedExport);

                return adaptedExport;
            }

            private void CheckAdaptation(Export adaptedExport)
            {
                // If an export was not provided, then the 
                // adapter could not adapt from the export.
                if (adaptedExport == null)
                {
                    return;
                }

                if (!StringComparers.ContractName.Equals(adaptedExport.Definition.ContractName, ToContractName))
                {   // The returned export must match the adapter's 'To' contract name

                    ICompositionElement element = Export.ToElement();
                    throw new CompositionException(CompositionError.Create(
                            CompositionErrorId.Adapter_ContractMismatch,
                            element,
                            Strings.Adapter_ContractMismatch,
                            element.DisplayName,
                            this.ToContractName,
                            adaptedExport.Definition.ContractName));
                }
            }

            private string GetContractName(string name)
            {
                // Similar to the ExportAttribute, we allow the contract name to be 
                // adapted to/from to be either specified as a type or as a string
                Type type = this.Export.Metadata.GetValue<Type>(name);
                if (type != null)
                {
                    return AttributedModelServices.GetContractName(type);
                }

                return this.Export.Metadata.GetValue<string>(name);
            }

            public bool CanAdaptFrom(string contractName)
            {
                // We have missing to/from contract names, say we can adapt, 
                // so that we throw an error during adaption alerting the user
                // of the fact.
                if (HasNullOrEmptyContracts())
                {
                    return true;
                }

                return StringComparers.ContractName.Equals(ToContractName, contractName);
            }

            public void EnsureWellFormedAdapter()
            {
                if (HasNullOrEmptyContracts())
                {   // Null or empty 'from' or 'to' contract

                    ICompositionElement element = Export.ToElement();
                    throw new CompositionException(CompositionError.Create(
                            CompositionErrorId.Adapter_CannotAdaptNullOrEmptyFromOrToContract,
                            element,
                            Strings.Adapter_CannotAdaptNullOrEmptyFromOrToContract,
                            element.DisplayName,
                            this.ToContractName));
                }

                if (StringComparers.ContractName.Equals(this.ToContractName, this.FromContractName))
                {   // 'From' and 'to' contracts match

                    ICompositionElement element = Export.ToElement();
                    throw new CompositionException(CompositionError.Create(
                            CompositionErrorId.Adapter_CannotAdaptFromAndToSameContract,
                            element,
                            Strings.Adapter_CannotAdaptFromAndToSameContract,
                            element.DisplayName,
                            this.ToContractName));
                }

                if (_adaptMethod == null)
                {
                    _adaptMethod = GetAdapterMethod();
                }
            }

            private Func<Export, Export> GetAdapterMethod()
            {
                // TODO: If the predictive composition feature
                // does not get implemented and hence this continues 
                // to throw, add additional context here.
                object value = this.Export.GetExportedObject();

                ExportedDelegate exportedDelegate = value as ExportedDelegate;
                if (exportedDelegate != null)
                {
                    value = exportedDelegate.CreateDelegate(typeof(Func<Export, Export>));
                }

                Func<Export, Export> method = value as Func<Export, Export>;
                if (method == null)
                {
                    ICompositionElement element = Export.ToElement();
                    throw new CompositionException(CompositionError.Create(
                            CompositionErrorId.Adapter_TypeMismatch,
                                element,
                                value == null ? Strings.Adapter_TypeNull : Strings.Adapter_TypeMismatch,
                                element.DisplayName,
                                value == null ? null : value.GetType().FullName,
                                typeof(Func<Export, Export>).FullName));

                }

                return method;
            }

            private bool HasNullOrEmptyContracts()
            {
                return String.IsNullOrEmpty(this.ToContractName) || String.IsNullOrEmpty(this.FromContractName);
            }
        }
    }
}
