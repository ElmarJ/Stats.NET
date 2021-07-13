// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;

namespace System.ComponentModel.Composition
{
    internal static class CompositionContainerExtensions
    {
        public static bool IsPresent<T>(this CompositionContainer container)
        {
            try
            {
                container.GetExportedObject<T>();
                return true;
            }
            catch (ImportCardinalityMismatchException)
            {
                return false;
            }
        }

        public static bool IsPresent(this CompositionContainer container, string contractName)
        {
            try
            {
                container.GetExportedObject<object>(contractName);
                return true;
            }
            catch (ImportCardinalityMismatchException)
            {
                return false;
            }
        }

        public static void AddAndComposeExportedObject<T>(this CompositionContainer container, T exportedObject)
        {
            var batch = new CompositionBatch();
            batch.AddExportedObject<T>(exportedObject);
            container.Compose(batch);
        }

        public static void AddAndComposeExportedObject<T>(this CompositionContainer container, string contractName, T exportedObject)
        {
            var batch = new CompositionBatch();
            batch.AddExportedObject<T>(contractName, exportedObject);
            container.Compose(batch);
        }

        public static void AddParts(this CompositionBatch batch, params object[] parts)
        {
            foreach (object instance in parts)
            {
                ComposablePart part = instance as ComposablePart;
                if (part != null)
                {
                    batch.AddPart(part);
                }
                else
                {
                    batch.AddPart((object)instance);
                }
            }
        }

        public static ComposablePart AddExportedObject(this CompositionBatch batch, string contractName, Type contractType, object exportedObject)
        {
            string typeIdentity = AttributedModelServices.GetTypeIdentity(contractType);

            IDictionary<string, object> metadata = null;

            if (typeIdentity != null)
            {
                metadata = new Dictionary<string, object>();
                metadata.Add(CompositionConstants.ExportTypeIdentityMetadataName, typeIdentity);
            }

            return batch.AddExport(new Export(contractName, metadata, () => exportedObject));
        }
    }
}
