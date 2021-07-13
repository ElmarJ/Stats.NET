﻿// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.AttributedModel;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;

namespace System.ComponentModel.Composition.Factories
{
    internal static class AdapterFactory
    {
        public static IDictionary<string, object> CreateAdapterMetadata(object fromContractNameOrType, object toContractNameOrType)
        {
            IDictionary<string, object> metadata = new Dictionary<string, object>();
            metadata[CompositionConstants.AdapterFromContractMetadataName] = fromContractNameOrType;
            metadata[CompositionConstants.AdapterToContractMetadataName] = toContractNameOrType;

            return metadata;
        }

        public static ComposablePart CreateAdapter(object fromContractNameOrType, object toContractNameOrType)
        {
            string contractName = ContractNameFromNameOrType(toContractNameOrType);

            return CreateAdapter(fromContractNameOrType, toContractNameOrType, export =>
                    {
                        return ExportFactory.Create(contractName,
                                                    export.Definition.Metadata,
                                                    () => export.GetExportedObject());
                    });
        }

        public static ComposablePart CreateAdapter(object fromContractNameOrType, object toContractNameOrType, Func<Export, Export> adapt)
        {
            var metadata = CreateAdapterMetadata(fromContractNameOrType, toContractNameOrType);

            return PartFactory.CreateExporter(new MicroExport(CompositionConstants.AdapterContractName, metadata, adapt));
        }

        public static string ContractNameFromNameOrType(object contractNameOrType)
        {
            string contractName = contractNameOrType as string;
            if (contractName != null)
            {
                return contractName;
            }

            Type contractType = contractNameOrType as Type;
            if (contractType != null)
            {
                return AttributedModelServices.GetContractName((Type)contractNameOrType);
            }
            
            return null;
        }
    }
}