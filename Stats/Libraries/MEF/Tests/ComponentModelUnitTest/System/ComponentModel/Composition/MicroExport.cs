// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;

namespace System.ComponentModel.Composition
{
    internal class MicroExport
    {
        public MicroExport(Type contractType, params object[] exportedObjects)
            : this(AttributedModelServices.GetContractName(contractType), contractType, (IDictionary<string, object>)null, exportedObjects)
        {
        }

        public MicroExport(string contractName, params object[] exportedObjects)
            : this(contractName, exportedObjects[0].GetType(), (IDictionary<string, object>)null, exportedObjects)
        {
        }

        public MicroExport(Type contractType, IDictionary<string, object> metadata, params object[] exportedObjects)
            : this(AttributedModelServices.GetContractName(contractType), exportedObjects[0].GetType(), metadata, exportedObjects)
        {
        }

        public MicroExport(string contractName, Type contractType, params object[] exportedObjects)
            : this(contractName, contractType, (IDictionary<string, object>)null, exportedObjects)
        {
        }

        public MicroExport(string contractName, IDictionary<string, object> metadata, params object[] exportedObjects)
            : this(contractName, exportedObjects[0].GetType(), metadata, exportedObjects)
        {
        }

        public MicroExport(string contractName, Type contractType, IDictionary<string, object> metadata, params object[] exportedObjects)
        {
            this.ContractName = contractName;
            this.ExportedObjects = exportedObjects;

            if (contractType != null)
            {
                string typeIdentity = AttributedModelServices.GetTypeIdentity(contractType);

                if (metadata == null)
                {
                    metadata = new Dictionary<string, object>();
                }

                object val;
                if (!metadata.TryGetValue(CompositionConstants.ExportTypeIdentityMetadataName, out val))
                {
                    metadata.Add(CompositionConstants.ExportTypeIdentityMetadataName, AttributedModelServices.GetTypeIdentity(contractType));
                }
            }
            this.Metadata = metadata;
        }

        public string ContractName
        {
            get;
            private set;
        }

        public object[] ExportedObjects
        {
            get;
            private set;
        }

        public IDictionary<string, object> Metadata
        {
            get;
            private set;
        }
    }
}