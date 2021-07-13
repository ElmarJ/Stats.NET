// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.ReflectionModel;
using System.Reflection;
using Microsoft.Internal;
using Microsoft.Internal.Collections;

namespace System.ComponentModel.Composition
{
    // Provides helpers for creating and dealing with Exports
    internal static partial class ExportServices
    {
        private static MethodInfo _createStronglyTypedExport = typeof(ExportServices).GetMethod("CreateStronglyTypedExport", BindingFlags.NonPublic | BindingFlags.Static);
        internal static readonly Type DefaultMetadataViewType = typeof(IDictionary<string, object>);
        internal static readonly Type DefaultExportedObjectType = typeof(object);

        internal static bool IsDefaultMetadataViewType(Type metadataViewType)
        {
            Assumes.NotNull(metadataViewType);

            // Consider all types that IDictionary<string, object> derives from, such
            // as ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>> 
            // and IEnumerable, as default metadata view
            return metadataViewType.IsAssignableFrom(DefaultMetadataViewType);
        }

        internal static bool IsDictionaryConstructorViewType(Type metadataViewType)
        {
            Assumes.NotNull(metadataViewType);

            // Does the view type have a constructor that is a Dictionary<string, object>
            return metadataViewType.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                                                    Type.DefaultBinder,
                                                    new Type[] { typeof(IDictionary<string, object>) },
                                                    new ParameterModifier[0]) != null;
        }

        internal static Func<Export, object> CreateStronglyTypedExportFactory(Type exportType, Type metadataViewType)
        {
            MethodInfo genericMethod = _createStronglyTypedExport.MakeGenericMethod(exportType, metadataViewType);
            return (Func<Export, object>)Delegate.CreateDelegate(typeof(Func<Export, object>), genericMethod);
        }

        internal static object CreateStronglyTypedExport<T, M>(Export export)
        {
            return new StructuredValueExport<T, M>(export);
        }
        
        internal static ExportCardinalityCheckResult CheckCardinality(ImportDefinition definition, IEnumerable<Export> exports)
        {
            EnumerableCardinality actualCardinality = exports.GetCardinality();

            switch (actualCardinality)
            {
                case EnumerableCardinality.Zero:
                    if (definition.Cardinality == ImportCardinality.ExactlyOne)
                    {
                        return ExportCardinalityCheckResult.NoExports;
                    }
                    break;

                case EnumerableCardinality.TwoOrMore:
                    if (definition.Cardinality.IsAtMostOne())
                    {
                        return ExportCardinalityCheckResult.TooManyExports;
                    }
                    break;

                default:
                    Assumes.IsTrue(actualCardinality == EnumerableCardinality.One);
                    break;

            }

            return ExportCardinalityCheckResult.Match;
        }
    }
}
