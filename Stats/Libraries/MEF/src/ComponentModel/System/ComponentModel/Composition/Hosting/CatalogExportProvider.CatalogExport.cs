// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.ComponentModel.Composition.Primitives;
using Microsoft.Internal;

namespace System.ComponentModel.Composition.Hosting
{
    public partial class CatalogExportProvider
    {
        private class CatalogExport : Export, IDisposable
        {
            private readonly CatalogExportProvider _catalogExportProvider;
            private readonly ComposablePartDefinition _partDefintion;
            private readonly ExportDefinition _definition;
            private readonly CreationPolicy _requiredCreationPolicy;
            private ComposablePart _part;
            private bool _isSharedPart;
            private object _exportedObject;

            public CatalogExport(CatalogExportProvider catalogExportProvider,
                ComposablePartDefinition partDefintion, ExportDefinition definition, CreationPolicy importCreationPolicy)
            {
                this._catalogExportProvider = catalogExportProvider;
                this._partDefintion = partDefintion;
                this._definition = definition;
                this._requiredCreationPolicy = importCreationPolicy;
            }

            public override ExportDefinition Definition
            {
                get
                {
                    return this._definition;
                }
            }

            protected override object GetExportedObjectCore()
            {
                if (this._exportedObject == null)
                {
                    CreationPolicy partPolicy = this._partDefintion.Metadata.GetValue<CreationPolicy>(CompositionConstants.PartCreationPolicyMetadataName);
                    this._isSharedPart = ShouldUseSharedPart(partPolicy, this._requiredCreationPolicy);

                    ComposablePart part = this._catalogExportProvider.GetComposablePart(this._partDefintion, this._isSharedPart);

                    this._exportedObject = this._catalogExportProvider.GetExportedObject(part, this._definition, this._isSharedPart);
                    this._part = part;
                }
                return this._exportedObject;
            }

            private static bool ShouldUseSharedPart(CreationPolicy partPolicy, CreationPolicy importPolicy)
            {
                // Matrix that details which policy to use for a given part to satisfy a given import.
                //                   Part.Any   Part.Shared  Part.NonShared
                // Import.Any        Shared     Shared       NonShared
                // Import.Shared     Shared     Shared       N/A
                // Import.NonShared  NonShared  N/A          NonShared

                switch (partPolicy)
                {
                    case CreationPolicy.Any:
                        {
                            if (importPolicy == CreationPolicy.Any ||
                                importPolicy == CreationPolicy.Shared)
                            {
                                return true;
                            }
                            return false;
                        }

                    case CreationPolicy.NonShared:
                        {
                            Assumes.IsTrue(importPolicy != CreationPolicy.Shared);
                            return false;
                        }

                    default:
                        {
                            Assumes.IsTrue(partPolicy == CreationPolicy.Shared);
                            Assumes.IsTrue(importPolicy != CreationPolicy.NonShared);
                            return true;
                        }
                }
            }

            void IDisposable.Dispose()
            {
                if (this._part != null && !this._isSharedPart)
                {
                    this._catalogExportProvider.ReleasePart(this._exportedObject, this._part);
                    this._part = null;
                }

                GC.SuppressFinalize(this);
            }
        }
    }
}
