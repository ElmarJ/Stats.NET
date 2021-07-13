// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.ComponentModel.Composition.Primitives;
using System.Globalization;
using Microsoft.Internal;

namespace System.ComponentModel.Composition
{
    partial class ExportServices
    {
        // Represents an Export that uses structured values underneath to cast the 
        // exported object to 'T'.
        private class StructuredValueExport<T, TMetadataView> : Export<T, TMetadataView>, IDisposable
        {
            private readonly Export _export;
            private object _exportedObject;

            public StructuredValueExport(Export export)
            {
                Assumes.NotNull(export);

                this._export = export;
            }

            public override ExportDefinition Definition
            {
                get { return this._export.Definition; }
            }

            protected override object GetExportedObjectCore()
            {
                if (this._exportedObject == null)
                {
                    object exportedObject = this._export.GetExportedObject();

                    bool succeeded = ContractServices.TryCast(typeof(T), exportedObject, out _exportedObject);
                    if(!succeeded)
                    {
                        throw new CompositionContractMismatchException(string.Format(CultureInfo.CurrentCulture,
                            Strings.ContractMismatch_ExportedObjectCannotBeCastToT,
                            this._export.ToElement().DisplayName,
                            typeof(T)));
                    }
                }

                return _exportedObject;
            }

            void IDisposable.Dispose()
            {
                IDisposable disposable = this._export as IDisposable;

                if (disposable != null)
                {
                    disposable.Dispose();
                }
                GC.SuppressFinalize(this);
            }
        }
    }
}
