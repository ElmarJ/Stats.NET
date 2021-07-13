// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using Microsoft.Internal;
using Microsoft.Internal.Collections;

namespace System.ComponentModel.Composition
{
    internal static class MetadataServices
    {
        public static readonly IDictionary<string, object> EmptyMetadata = new ReadOnlyDictionary<string, object>(null);

        public static IDictionary<string, object> AsReadOnly(this IDictionary<string, object> metadata)
        {
            if (metadata == null)
            {
                return EmptyMetadata;
            }

            if (metadata is ReadOnlyDictionary<string, object>)
            {
                return metadata;
            }

            return new ReadOnlyDictionary<string, object>(metadata);
        }

        public static IDictionary<string, object> SetValue(this IDictionary<string, object> metadata, string key, object value)
        {
            IDictionary<string, object> writeableMetadata = null; 
            ReadOnlyDictionary<string, object> readOnlyMetadata = metadata as ReadOnlyDictionary<string, object>;
            if (metadata == MetadataServices.EmptyMetadata)
            {
                writeableMetadata = new Dictionary<string, object>();
                metadata = writeableMetadata.AsReadOnly();
            }
            else if (readOnlyMetadata != null)
            {
                writeableMetadata = readOnlyMetadata.InnerDictionary;
            }
            else
            {
                writeableMetadata = metadata;
            }
            writeableMetadata[key] = value;

            return metadata;
        }

        public static T GetValue<T>(this IDictionary<string, object> metadata, string key)
        {
            Assumes.NotNull(metadata, "metadata");

            object untypedValue = true;
            if (!metadata.TryGetValue(key, out untypedValue))
            {
                return default(T);
            }

            if (untypedValue is T)
            {
                return (T)untypedValue;
            }
            else
            {
                return default(T);
            }
        }
    }
}
