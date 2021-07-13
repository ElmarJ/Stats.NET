// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Primitives;

namespace System.ComponentModel.Composition.Factories
{
    // This class deliberately does not create instances of Export<T, TMetadataView>,
    // so as to test other derived classes from Export<T, TMetadataView>.
    internal static partial class ExportFactory
    {
        public static IEnumerable<Export> Create(string contractName, int count)
        {
            Export[] exports = new Export[count];

            for (int i = 0; i < count; i++)
            {
                exports[i] = Create(contractName, (object)null);
            }

            return exports;
        }

        public static Export<T> Create<T>(string contractName, T value)
        {
            return Create<T, object>(contractName, (IDictionary<string, object>)null, () => value);
        }

        public static Export<T> Create<T>(string contractName, T value, IDictionary<string, object> metadata)
        {
            return Create<T, object>(contractName, metadata, () => value);
        }

        public static Export Create(string contractName, IDictionary<string, object> metadata, Func<object> exportedObjectGetter)
        {
            return Create<object, object>(contractName, metadata, exportedObjectGetter);
        }

        public static Export Create(string contractName, Func<object> exportedObjectGetter)
        {
            return Create<object, object>(contractName, (IDictionary<string, object>)null, exportedObjectGetter);
        }

        public static Export Create(string contractName, object value)
        {
            return Create<object, object>(contractName, (IDictionary<string, object>)null, () => value);
        }

        public static Export Create(string contractName, IDictionary<string, object> metadata, object value)
        {
            return Create<object, object>(contractName, metadata, () => value);
        }

        private static Export<T, TMetadataView> Create<T, TMetadataView>(string contractName, IDictionary<string, object> metadata, Func<T> exportedObjectGetter)
        {
            var definition = ExportDefinitionFactory.Create(contractName, metadata);

            return new DerivedExport<T, TMetadataView>(definition, exportedObjectGetter);
        }
    }
}
