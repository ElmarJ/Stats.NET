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
using System.ComponentModel.Composition.ReflectionModel;
using System.ComponentModel.Composition.Factories;
using System.ComponentModel.Composition.Primitives;
using Microsoft.Internal.Collections;

namespace System.ComponentModel.Composition.Caching.AttributedModel
{
    public class PartWithExportWithMetadata
    {
        [Export("FooExport1")]
        [ExportMetadata("Foo1", "Bar")]
        [ExportMetadata("Number", 42)]
        public object FooExport1 { get; set; }
    }

    public class PartWithExportNoMetadata
    {
        [Export("FooExport1")]
        public object FooExport1 { get; set; }
    }

    public class PartWithSimpleImport
    {
        [Import("FooImport1", AllowDefault=true)]
        public object FooImport { get; set; }
    }

    public class PartWithConstructor
    {
        [ImportingConstructor]
        public PartWithConstructor([Import("Foo")] object foo)
        {
        }
    }

    public class PartWithSimpleImportSetterOnly
    {
        [Import("FooImport1", AllowDefault = true)]
        public object FooImport { set { } }
    }

    public class PartWithImportRequiredMetadata
    {
        [ImportMany("FooImport1")]
        public ExportCollection<object, IFooMetadataView> FooImport { get; set; }
    }

    [CompositionOptions(CreationPolicy = CreationPolicy.NonShared)]
    public class PartWithImportsAndExports
    {
        [ImportingConstructor]
        public PartWithImportsAndExports([Import("FooImport1")] object arg1, [Import("FooImport2")] object arg2)
        {
        }

        [Export("FooExport1")]
        [ExportMetadata("Foo1", "Bar")]
        [ExportMetadata("Number", 42)]
        public object FooExport1 { get; set; }

        [Import("FooImport1", AllowDefault = true)]
        public object FooImport { get; set; }

    }

    [TestClass]
    public class AttributedComposablePartCatalogSiteTests
    {
        [TestMethod]
        public void CacheExportDefinition()
        {
            ReflectionComposablePartDefinition partDefinition = PartDefinitionFactory.CreateAttributed(typeof(PartWithExportWithMetadata));
            ExportDefinition export = partDefinition.ExportDefinitions.First();

            AttributedComposablePartCatalogSite catalogSite = new AttributedComposablePartCatalogSite();
            IDictionary<string, object> cache = catalogSite.CacheExportDefinition(partDefinition, export);

            Assert.IsNotNull(cache);
            Assert.AreEqual(4, cache.Count);

            Assert.AreEqual(export.ContractName, cache[AttributedCacheServices.CacheKeys.ContractName]);
            Assert.AreEqual(MemberTypes.Property, cache[AttributedCacheServices.CacheKeys.MemberType]);
            EnumerableAssert.AreEqual(export.Metadata, (IDictionary<string, object>)cache[AttributedCacheServices.CacheKeys.Metadata]);
        }

        [TestMethod]
        public void CacheExportDefinition_NoMetadata()
        {
            ReflectionComposablePartDefinition partDefinition = PartDefinitionFactory.CreateAttributed(typeof(PartWithExportNoMetadata));
            ExportDefinition export = partDefinition.ExportDefinitions.First();

            AttributedComposablePartCatalogSite catalogSite = new AttributedComposablePartCatalogSite();
            IDictionary<string, object> cache = catalogSite.CacheExportDefinition(partDefinition, export);

            Assert.IsNotNull(cache);
            Assert.AreEqual(4, cache.Count); // metadata containts type identity so metadata is also now included

            Assert.AreEqual(export.ContractName, cache[AttributedCacheServices.CacheKeys.ContractName]);
            Assert.AreEqual(MemberTypes.Property, cache[AttributedCacheServices.CacheKeys.MemberType]);
        }

        [TestMethod]
        public void CreateExportDefinitionFromCache()
        {
            ReflectionComposablePartDefinition partDefinition = PartDefinitionFactory.CreateAttributed(typeof(PartWithExportWithMetadata));
            ExportDefinition export = partDefinition.ExportDefinitions.First();

            AttributedComposablePartCatalogSite catalogSite = new AttributedComposablePartCatalogSite();
            IDictionary<string, object> cache = catalogSite.CacheExportDefinition(partDefinition, export);

            ExportDefinition cachedExport = catalogSite.CreateExportDefinitionFromCache(partDefinition, cache);

            Assert.IsNotNull(cachedExport);
            Assert.AreEqual(export.ContractName, cachedExport.ContractName);
            EnumerableAssert.AreEqual(export.Metadata, cachedExport.Metadata);

        }

        [TestMethod]
        public void CreateExportDefinitionFromCache_NoMetadata()
        {
            ReflectionComposablePartDefinition partDefinition = PartDefinitionFactory.CreateAttributed(typeof(PartWithExportNoMetadata));
            ExportDefinition export = partDefinition.ExportDefinitions.First();

            AttributedComposablePartCatalogSite catalogSite = new AttributedComposablePartCatalogSite();
            IDictionary<string, object> cache = catalogSite.CacheExportDefinition(partDefinition, export);

            ExportDefinition cachedExport = catalogSite.CreateExportDefinitionFromCache(partDefinition, cache);

            Assert.IsNotNull(cachedExport);
            Assert.AreEqual(export.ContractName, cachedExport.ContractName);
            EnumerableAssert.AreEqual(export.Metadata, cachedExport.Metadata);
        }

        [TestMethod]
        public void CacheImportDefinition()
        {
            var partDefinition = PartDefinitionFactory.CreateAttributed(typeof(PartWithSimpleImport));
            var import = (ContractBasedImportDefinition)partDefinition.ImportDefinitions.First();

            var catalogSite = new AttributedComposablePartCatalogSite();
            var cache = catalogSite.CacheImportDefinition(partDefinition, import);

            Assert.AreEqual(3, cache.Count);
            Assert.AreEqual(import.ContractName, cache[AttributedCacheServices.CacheKeys.ContractName]);
            Assert.AreEqual(2, ((IDictionary<string, object>)cache[AttributedCacheServices.CacheKeys.Accessors]).Count);
        }

        [TestMethod]
        public void CacheImportDefinition_SetterOnly()
        {
            var partDefinition = PartDefinitionFactory.CreateAttributed(typeof(PartWithSimpleImportSetterOnly));
            var import = (ContractBasedImportDefinition)partDefinition.ImportDefinitions.First();

            var catalogSite = new AttributedComposablePartCatalogSite();
            var cache = catalogSite.CacheImportDefinition(partDefinition, import);

            Assert.AreEqual(3, cache.Count);
            Assert.AreEqual(import.ContractName, cache[AttributedCacheServices.CacheKeys.ContractName]);
            Assert.AreEqual(2, ((IDictionary<string, object>)cache[AttributedCacheServices.CacheKeys.Accessors]).Count);
        }

        [TestMethod]
        public void CacheImportDefinition_ConstructorParameter()
        {
            var partDefinition = PartDefinitionFactory.CreateAttributed(typeof(PartWithConstructor));
            var import = (ContractBasedImportDefinition)partDefinition.ImportDefinitions.First();

            var catalogSite = new AttributedComposablePartCatalogSite();
            var cache = catalogSite.CacheImportDefinition(partDefinition, import);

            Assert.AreEqual(3, cache.Count);
            Assert.AreEqual(import.ContractName, cache[AttributedCacheServices.CacheKeys.ContractName]);
            Assert.AreEqual(AttributedCacheServices.ImportTypes.Parameter, cache[AttributedCacheServices.CacheKeys.ImportType]);
            Assert.AreEqual(2, ((IDictionary<string, object>)cache[AttributedCacheServices.CacheKeys.Parameter]).Count);
        }

        [TestMethod]
        public void CacheImportDefinition_RequiredMetadata()
        {
            var partDefinition = PartDefinitionFactory.CreateAttributed(typeof(PartWithImportRequiredMetadata));
            var import = (ContractBasedImportDefinition)partDefinition.ImportDefinitions.First();

            var catalogSite = new AttributedComposablePartCatalogSite();
            var cache = catalogSite.CacheImportDefinition(partDefinition, import);

            Assert.AreEqual(4, cache.Count);
            Assert.AreEqual(import.ContractName, cache[AttributedCacheServices.CacheKeys.ContractName]);
            Assert.AreEqual(import.Cardinality, cache[AttributedCacheServices.CacheKeys.Cardinality]);
            IEnumerable<string> requiredMetadata = (IEnumerable<string>)cache[AttributedCacheServices.CacheKeys.RequiredMetadata];
            Assert.AreEqual(1, requiredMetadata.Count());
            Assert.AreEqual("Foo", requiredMetadata.First());
        }

        [TestMethod]
        public void CreateImportDefinitionFromCache()
        {
            var partDefinition = PartDefinitionFactory.CreateAttributed(typeof(PartWithSimpleImport));
            var import = (ContractBasedImportDefinition)partDefinition.ImportDefinitions.First();

            AttributedComposablePartCatalogSite catalogSite = new AttributedComposablePartCatalogSite();
            IDictionary<string, object> cache = catalogSite.CacheImportDefinition(partDefinition, import);
            var cachedImport = (ContractBasedImportDefinition)catalogSite.CreateImportDefinitionFromCache(partDefinition, cache);

            Assert.IsNotNull(cachedImport);
            Assert.AreEqual(import.ContractName, cachedImport.ContractName);
            Assert.AreEqual(import.IsPrerequisite, cachedImport.IsPrerequisite);
            Assert.AreEqual(import.Cardinality, cachedImport.Cardinality);
            Assert.AreEqual(import.IsRecomposable, cachedImport.IsRecomposable);

            string contractName;
            IEnumerable<string> requiredMetadata;
            string cachedContractName;
            IEnumerable<string> cachedRequiredMetadata;

            Assert.IsTrue(ContraintParser.TryParseConstraint(import.Constraint, out contractName, out requiredMetadata));
            Assert.IsTrue(ContraintParser.TryParseConstraint(cachedImport.Constraint, out cachedContractName, out cachedRequiredMetadata));

            Assert.AreEqual(contractName, cachedContractName);
            Assert.IsTrue(requiredMetadata.SequenceEqual(cachedRequiredMetadata));

            ReflectionMemberImportDefinition reflectionImport = cachedImport as ReflectionMemberImportDefinition;
            Assert.IsNotNull(reflectionImport);
            Assert.IsNotNull(reflectionImport.ImportingLazyMember.GetAccessors());
        }

        [TestMethod]
        public void CreateImportDefinitionFromCache_SetterOnly()
        {
            var partDefinition = PartDefinitionFactory.CreateAttributed(typeof(PartWithSimpleImportSetterOnly));
            var import = (ContractBasedImportDefinition)partDefinition.ImportDefinitions.First();

            AttributedComposablePartCatalogSite catalogSite = new AttributedComposablePartCatalogSite();
            IDictionary<string, object> cache = catalogSite.CacheImportDefinition(partDefinition, import);
            var cachedImport = (ContractBasedImportDefinition)catalogSite.CreateImportDefinitionFromCache(partDefinition, cache);

            Assert.IsNotNull(cachedImport);
            Assert.AreEqual(import.ContractName, cachedImport.ContractName);
            Assert.AreEqual(import.IsPrerequisite, cachedImport.IsPrerequisite);
            Assert.AreEqual(import.Cardinality, cachedImport.Cardinality);
            Assert.AreEqual(import.IsRecomposable, cachedImport.IsRecomposable);

            string contractName;
            IEnumerable<string> requiredMetadata;
            string cachedContractName;
            IEnumerable<string> cachedRequiredMetadata;

            Assert.IsTrue(ContraintParser.TryParseConstraint(import.Constraint, out contractName, out requiredMetadata));
            Assert.IsTrue(ContraintParser.TryParseConstraint(cachedImport.Constraint, out cachedContractName, out cachedRequiredMetadata));

            Assert.AreEqual(contractName, cachedContractName);
            Assert.IsTrue(requiredMetadata.SequenceEqual(cachedRequiredMetadata));

            ReflectionMemberImportDefinition reflectionImport = cachedImport as ReflectionMemberImportDefinition;
            Assert.IsNotNull(reflectionImport);
            Assert.IsNotNull(reflectionImport.ImportingLazyMember.GetAccessors());
        }

        [TestMethod]
        public void CreateImportDefinitionFromCache_ConstructorParameter()
        {
            var partDefinition = PartDefinitionFactory.CreateAttributed(typeof(PartWithConstructor));
            var import = (ContractBasedImportDefinition)partDefinition.ImportDefinitions.First();

            AttributedComposablePartCatalogSite catalogSite = new AttributedComposablePartCatalogSite();
            IDictionary<string, object> cache = catalogSite.CacheImportDefinition(partDefinition, import);
            var cachedImport = (ContractBasedImportDefinition)catalogSite.CreateImportDefinitionFromCache(partDefinition, cache);

            Assert.IsNotNull(cachedImport);
            Assert.AreEqual(import.ContractName, cachedImport.ContractName);
            Assert.AreEqual(import.IsPrerequisite, cachedImport.IsPrerequisite);
            Assert.AreEqual(import.Cardinality, cachedImport.Cardinality);
            Assert.AreEqual(import.IsRecomposable, cachedImport.IsRecomposable);

            string contractName;
            IEnumerable<string> requiredMetadata;
            string cachedContractName;
            IEnumerable<string> cachedRequiredMetadata;

            Assert.IsTrue(ContraintParser.TryParseConstraint(import.Constraint, out contractName, out requiredMetadata));
            Assert.IsTrue(ContraintParser.TryParseConstraint(cachedImport.Constraint, out cachedContractName, out cachedRequiredMetadata));

            Assert.AreEqual(contractName, cachedContractName);
            Assert.IsTrue(requiredMetadata.SequenceEqual(cachedRequiredMetadata));

            ReflectionParameterImportDefinition reflectionImport = cachedImport as ReflectionParameterImportDefinition;
            Assert.IsNotNull(reflectionImport);
            Assert.IsNotNull(reflectionImport.ImportingLazyParameter.Value);
        }

        [TestMethod]
        public void CreateImportDefinitionFromCache_RequiredMetadata()
        {
            var partDefinition = PartDefinitionFactory.CreateAttributed(typeof(PartWithImportRequiredMetadata));
            var import = (ContractBasedImportDefinition)partDefinition.ImportDefinitions.First();

            var catalogSite = new AttributedComposablePartCatalogSite();
            var cache = catalogSite.CacheImportDefinition(partDefinition, import);
            var cachedImport = (ContractBasedImportDefinition)catalogSite.CreateImportDefinitionFromCache(partDefinition, cache);

            Assert.IsNotNull(cachedImport);
            Assert.AreEqual(import.ContractName, cachedImport.ContractName);
            Assert.AreEqual(import.IsPrerequisite, cachedImport.IsPrerequisite);
            Assert.AreEqual(import.Cardinality, cachedImport.Cardinality);
            Assert.AreEqual(import.IsRecomposable, cachedImport.IsRecomposable);

            string contractName;
            IEnumerable<string> requiredMetadata;
            string cachedContractName;
            IEnumerable<string> cachedRequiredMetadata;

            Assert.IsTrue(ContraintParser.TryParseConstraint(import.Constraint, out contractName, out requiredMetadata));
            Assert.IsTrue(ContraintParser.TryParseConstraint(cachedImport.Constraint, out cachedContractName, out cachedRequiredMetadata));

            Assert.AreEqual(contractName, cachedContractName);
            Assert.IsTrue(requiredMetadata.SequenceEqual(cachedRequiredMetadata));
        }

        [TestMethod]
        public void CacheImportDefinition_CacheTwice()
        {
            ReflectionComposablePartDefinition partDefinition = PartDefinitionFactory.CreateAttributed(typeof(PartWithSimpleImport));
            ImportDefinition import = partDefinition.ImportDefinitions.First();

            AttributedComposablePartCatalogSite catalogSite = new AttributedComposablePartCatalogSite();
            IDictionary<string, object> cache = catalogSite.CacheImportDefinition(partDefinition, import);
            ImportDefinition cachedImport = catalogSite.CreateImportDefinitionFromCache(partDefinition, cache);
            IDictionary<string, object> cachedImportCache = catalogSite.CacheImportDefinition(partDefinition, cachedImport);
            Assert.IsTrue(cachedImportCache.DictionaryEquals(cache));
        }

        [TestMethod]
        public void CachePartDefinition()
        {
            ReflectionComposablePartDefinition partDefinition = PartDefinitionFactory.CreateAttributed(typeof(PartWithImportsAndExports));

            AttributedComposablePartCatalogSite catalogSite = new AttributedComposablePartCatalogSite();
            IDictionary<string, object> cache = catalogSite.CachePartDefinition(partDefinition);

            Assert.AreEqual(3, cache.Count);

            var assemblyCache = (IDictionary<string, object>)cache[AttributedCacheServices.CacheKeys.Assembly];

            Assert.AreEqual(partDefinition.GetPartType().Assembly.FullName, assemblyCache[AttributedCacheServices.CacheKeys.AssemblyFullName]);
            Assert.AreEqual(new Uri(partDefinition.GetPartType().Assembly.CodeBase).LocalPath, assemblyCache[AttributedCacheServices.CacheKeys.AssemblyLocation]);
            Assert.AreEqual(File.GetLastWriteTimeUtc(new Uri(partDefinition.GetPartType().Assembly.CodeBase).LocalPath).Ticks, (long)assemblyCache[AttributedCacheServices.CacheKeys.AssemblyTimeStamp]);
            EnumerableAssert.AreEqual(partDefinition.Metadata, (IDictionary<string, object>)cache[AttributedCacheServices.CacheKeys.Metadata]);
        }

        [TestMethod]
        [Ignore]
        public void CreatePartDefinitionFromCache()
        {
            ReflectionComposablePartDefinition partDefinition = PartDefinitionFactory.CreateAttributed(typeof(PartWithImportsAndExports));

            AttributedComposablePartCatalogSite catalogSite = new AttributedComposablePartCatalogSite();
            IDictionary<string, object> cache = catalogSite.CachePartDefinition(partDefinition);

            bool importsCreated = false;
            bool exportsCreated = false;

            ComposablePartDefinition cachedPartDefinition = catalogSite.CreatePartDefinitionFromCache(
                cache, 
                delegate
                {
                    Assert.IsFalse(importsCreated);
                    importsCreated = true;
                    return partDefinition.ImportDefinitions;
                },
                delegate
                {
                    Assert.IsFalse(exportsCreated);
                    exportsCreated = true;
                    return partDefinition.ExportDefinitions;
                });

            EnumerableAssert.AreEqual(partDefinition.ImportDefinitions, cachedPartDefinition.ImportDefinitions);
            Assert.IsTrue(importsCreated);
            EnumerableAssert.AreEqual(partDefinition.ImportDefinitions, cachedPartDefinition.ImportDefinitions);
            EnumerableAssert.AreEqual(partDefinition.ExportDefinitions, cachedPartDefinition.ExportDefinitions);
            Assert.IsTrue(exportsCreated);
            EnumerableAssert.AreEqual(partDefinition.ExportDefinitions, cachedPartDefinition.ExportDefinitions);
            EnumerableAssert.AreEqual(partDefinition.Metadata, cachedPartDefinition.Metadata);

        }

        [TestMethod]
        public void CreatePartDefinitionFromCache_CreatePart()
        {
            ReflectionComposablePartDefinition partDefinition = PartDefinitionFactory.CreateAttributed(typeof(PartWithImportsAndExports));

            AttributedComposablePartCatalogSite catalogSite = new AttributedComposablePartCatalogSite();
            IDictionary<string, object> cache = catalogSite.CachePartDefinition(partDefinition);

            bool importsCreated = false;
            bool exportsCreated = false;

            ComposablePartDefinition cachedPartDefinition = catalogSite.CreatePartDefinitionFromCache(
                cache,
                delegate
                {
                    Assert.IsFalse(importsCreated);
                    importsCreated = true;
                    return partDefinition.ImportDefinitions;
                },
                delegate
                {
                    Assert.IsFalse(exportsCreated);
                    exportsCreated = true;
                    return partDefinition.ExportDefinitions;
                });

            ReflectionComposablePart part = cachedPartDefinition.CreatePart() as ReflectionComposablePart;
            Assert.IsNotNull(part);
            Assert.AreEqual(part.Definition.GetPartType(), partDefinition.GetPartType());
        }

        [TestMethod]
        public void CreatePartDefinitionFromCache_CacheTwice()
        {
            ReflectionComposablePartDefinition partDefinition = PartDefinitionFactory.CreateAttributed(typeof(PartWithImportsAndExports));

            AttributedComposablePartCatalogSite catalogSite = new AttributedComposablePartCatalogSite();
            IDictionary<string, object> cache = catalogSite.CachePartDefinition(partDefinition);

            ComposablePartDefinition cachedPartDefinition = catalogSite.CreatePartDefinitionFromCache(cache, (part) => Enumerable.Empty<ImportDefinition>(), (part) => Enumerable.Empty<ExportDefinition>());
            IDictionary<string, object> cachedPartCache = catalogSite.CachePartDefinition(cachedPartDefinition);
            Assert.IsTrue(cache.DictionaryEquals(cachedPartCache));
        }
    }
}
