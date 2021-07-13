// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection.Emit;
using System.Reflection;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Caching;
using System.Collections;
using System.IO;
using System.Linq.Expressions;
using System.UnitTesting;
using System.ComponentModel.Composition.AttributedModel;
using System.ComponentModel.Composition.Factories;
using System.ComponentModel.Composition.Primitives;

namespace System.ComponentModel.Composition.Caching
{
    public class ConcreteCachedComposablePartCatalogSite : ICachedComposablePartCatalogSite
    {
        public ConcreteCachedComposablePartCatalogSite()
        {
        }

        public IDictionary<string, object> CacheExportDefinition(ComposablePartDefinition owner, ExportDefinition export)
        {
            IDictionary<string, object> cache = new Dictionary<string, object>();
            this.SetValue(cache, "ContractName", export.ContractName);
            this.SetValue(cache, "Metadata", export.Metadata);
            return cache;
        }

        public IDictionary<string, object> CacheImportDefinition(ComposablePartDefinition owner, ImportDefinition import)
        {
            var contractBasedImport = (ContractBasedImportDefinition)import;
            IDictionary<string, object> cache = new Dictionary<string, object>();
            this.SetValue(cache, "ContractName", contractBasedImport.ContractName);
            this.SetValue(cache, "IsPrerequisite", contractBasedImport.IsPrerequisite);
            this.SetValue(cache, "IsRecomposable", contractBasedImport.IsRecomposable);
            this.SetValue(cache, "Cardinality", contractBasedImport.Cardinality);
            return cache;
        }

        public IDictionary<string, object> CachePartDefinition(ComposablePartDefinition partDefinition)
        {
            IDictionary<string, object> cache = new Dictionary<string, object>();
            this.SetValue(cache, "Metadata", partDefinition.Metadata);
            return cache;
        }

        public ExportDefinition CreateExportDefinitionFromCache(ComposablePartDefinition owner, IDictionary<string, object> cache)
        {
            return ExportDefinitionFactory.Create(
                this.GetValue<string>(cache, "ContractName"),
                this.GetValue<IDictionary<string, object>>(cache, "Metadata"));
        }

        public ImportDefinition CreateImportDefinitionFromCache(ComposablePartDefinition owner, IDictionary<string, object> cache)
        {
            return ImportDefinitionFactory.Create(
                this.GetValue<string>(cache, "ContractName"),
                this.GetValue<ImportCardinality>(cache, "Cardinality"),
                this.GetValue<bool>(cache, "IsRecomposable"),
                this.GetValue<bool>(cache, "IsPrerequisite"));
        }

        public ComposablePartDefinition CreatePartDefinitionFromCache(IDictionary<string, object> cache, Func<ComposablePartDefinition, IEnumerable<ImportDefinition>> importsGetter, Func<ComposablePartDefinition, IEnumerable<ExportDefinition>> exportsGetter)
        {
            ComposablePartDefinition definition = null;
            definition =  PartDefinitionFactory.Create(
                this.GetValue<IDictionary<string, object>>(cache, "Metadata"),
                () => null,
                () => importsGetter(definition),
                () => exportsGetter(definition));
            return definition;
        }

        private void SetValue<T>(IDictionary<string, object> cache, string key, T value)
        {
            IDictionary<string, object> valueAsDictionary = value as IDictionary<string, object>;
            if ((valueAsDictionary != null) && (valueAsDictionary.Count == 0))
            {
                return;
            }

            if (object.Equals(value, default(T)))
            {
                return;
            }

            cache[key] = value;
        }

        private T GetValue<T>(IDictionary<string, object> cache, string key)
        {
            object value = null;
            if (cache.TryGetValue(key, out value))
            {
                return (T)value;
            }
            else
            {
                if (typeof(IDictionary<string, object>) == typeof(T))
                {
                    return (T)(object)MetadataServices.EmptyMetadata;
                }
                else
                {
                    return default(T);
                }
            }
        }
    }
}
