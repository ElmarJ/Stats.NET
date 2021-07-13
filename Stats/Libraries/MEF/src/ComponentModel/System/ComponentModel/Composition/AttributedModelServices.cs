// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Runtime.Serialization;
using System.ComponentModel.Composition.AttributedModel;
using System.Reflection;
using System.Linq;
using Microsoft.Internal;
using System.Collections.Generic;

namespace System.ComponentModel.Composition
{
    public static class AttributedModelServices
    {
        public static ComposablePart CreatePart(object attributedPart)
        {
            Requires.NotNull(attributedPart, "attributedPart");
            return AttributedModelDiscovery.CreatePart(attributedPart);
        }

        public static ComposablePartDefinition CreatePartDefinition(Type type, ICompositionElement origin)
        {
            Requires.NotNull(type, "type");
            return AttributedModelDiscovery.CreatePartDefinition(type, null, false, origin);
        }

        public static string GetTypeIdentity(Type type)
        {
            Requires.NotNull(type, "type");

            return ContractNameServices.GetDefaultContractName(type);
        }

        public static string GetTypeIdentity(MethodInfo method)
        {
            Requires.NotNull(method, "method");

            return ContractNameServices.GetContractNameFromMethod(method);
        }

        public static string GetContractName(Type type)
        {
            Requires.NotNull(type, "type");

            return CompositionServices.GetContractNameFromContractTypeAttribute(type) ?? ContractNameServices.GetDefaultContractName(type);
        }

        public static ComposablePart AddExportedObject<T>(this CompositionBatch batch, T exportedObject)
        {
            Requires.NotNull(batch, "batch");
            string contractName = AttributedModelServices.GetContractName(typeof(T));

            return batch.AddExportedObject<T>(contractName, exportedObject);
        }

        public static void ComposeExportedObject<T>(this CompositionContainer container, T exportedObject)
        {
            Requires.NotNull(container, "container");

            CompositionBatch batch = new CompositionBatch();
            batch.AddExportedObject<T>(exportedObject);
            container.Compose(batch);
        }

        public static ComposablePart AddExportedObject<T>(this CompositionBatch batch, string contractName, T exportedObject)
        {
            Requires.NotNull(batch, "batch");

            string typeIdentity = AttributedModelServices.GetTypeIdentity(typeof(T));

            IDictionary<string, object> metadata = new Dictionary<string, object>();
            metadata.Add(CompositionConstants.ExportTypeIdentityMetadataName, typeIdentity);

            return batch.AddExport(new Export(contractName, metadata, () => exportedObject));
        }

        public static void ComposeExportedObject<T>(this CompositionContainer container, string contractName, T exportedObject)
        {
            Requires.NotNull(container, "container");

            CompositionBatch batch = new CompositionBatch();
            batch.AddExportedObject<T>(contractName, exportedObject);
            container.Compose(batch);
        }

        public static ComposablePart AddPart(this CompositionBatch batch, object attributedPart)
        {
            Requires.NotNull(batch, "batch");
            Requires.NotNull(attributedPart, "attributedPart");

            ComposablePart part = AttributedModelServices.CreatePart(attributedPart);

            batch.AddPart(part);

            return part;
        }

        public static void ComposeParts(this CompositionContainer container, params object[] attributedParts)
        {
            Requires.NotNull(container, "container");
            Requires.NotNullOrNullElements(attributedParts, "attributedParts");

            CompositionBatch batch = new CompositionBatch(
                attributedParts.Select(attributedPart => AttributedModelServices.CreatePart(attributedPart)).ToArray(),
                Enumerable.Empty<ComposablePart>());

            container.Compose(batch);
        }
    }
}
