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
using System.ComponentModel.Composition.Primitives;
using System.Collections;
using System.UnitTesting;
using System.ComponentModel.Composition.AttributedModel;
using System.ComponentModel.Composition.Caching.AttributedModel;
using System.ComponentModel.Composition.Factories;

namespace System.ComponentModel.Composition.Caching
{
    [TestClass]
    public class ComposablePartCatalogAssemblyCacheTests
    {
        public class MyPart1
        {
            [Import("FooImport1")]
            public object FooImport { get; set; }

            [Export("FooExport1")]
            [ExportMetadata("Foo1", "Bar")]
            [ExportMetadata("Number", 42)]
            public object FooExport1 { get; set; }
        }

        public class MyPart2
        {
            [Import("FooImport2")]
            public object FooImport { get; set; }

            [Export("FooExport1")]
            [ExportMetadata("Foo2", "Bar")]
            [ExportMetadata("Number", 42)]
            public object FooExport1 { get; set; }

            [Export("FooExport2")]
            [ExportMetadata("Foo2", "Bar")]
            [ExportMetadata("Number", 42)]
            public object FooExport2 { get; set; }

        }

        public class MyPart3
        {
            [Import("FooImport")]
            public object FooImport { get; set; }

            [Export("FooExport1")]
            [ExportMetadata("Foo3", "Bar")]
            [ExportMetadata("Number", 42)]
            public object FooExport1 { get; set; }

            [Export("FooExport2")]
            [ExportMetadata("Foo3", "Bar")]
            [ExportMetadata("Number", 42)]
            public object FooExport2 { get; set; }

            [Export("FooExport3")]
            [ExportMetadata("Foo3", "Bar")]
            [ExportMetadata("Number", 42)]
            public object FooExport3 { get; set; }

        }

        private class MockReader : ComposablePartCatalogCacheReader
        {
            protected override ComposablePartCatalogCache ReadCacheCore(object cachedCatalogToken)
            {
                throw new NotImplementedException();
            }

            protected override object RootCacheToken
            {
                get { throw new NotImplementedException(); }
            }
        }

        [TestMethod]
        public void VerifyReader()
        {
            AssemblyCacheGenerator generator = AssemblyCacheGeneratorTests.BeginGeneration("Foo");
            Type stubType = AssemblyCacheGeneratorTests.EndGeneration(generator);

            ComposablePartCatalogCacheReader reader = new MockReader();
            ComposablePartCatalogAssemblyCache cache = new ComposablePartCatalogAssemblyCache(stubType, reader);

            Assert.AreSame(reader, cache.Reader);
        }

        [TestMethod]
        [Ignore]
        [WorkItem(507696)]
        public void VerifyMetadata()
        {
            IDictionary<string, object> expectedMetadata = new Dictionary<string, object>();
            expectedMetadata.Add("A", 42);
            expectedMetadata.Add("B", "Foo");
            expectedMetadata.Add("C", BindingFlags.ExactBinding);
            expectedMetadata.Add("D", this.GetType());

            AssemblyCacheGenerator generator = AssemblyCacheGeneratorTests.BeginGeneration("Foo");
            generator.CacheCatalogMetadata(expectedMetadata);
            Type stubType = AssemblyCacheGeneratorTests.EndGeneration(generator);

            ComposablePartCatalogAssemblyCache cache = new ComposablePartCatalogAssemblyCache(stubType, new MockReader());
            ComposablePartCatalog catalogCache = cache.GetCacheCatalog(new ConcreteCachedComposablePartCatalogSite());

            EnumerableAssert.AreEqual(expectedMetadata, cache.Metadata);
        }

        [TestMethod]
        public void ReadEmptyCache()
        {
            AssemblyCacheGenerator generator = AssemblyCacheGeneratorTests.BeginGeneration("Foo");
            Type stubType = AssemblyCacheGeneratorTests.EndGeneration(generator);

            ComposablePartCatalogAssemblyCache cache = new ComposablePartCatalogAssemblyCache(stubType, new MockReader());
            ComposablePartCatalog catalogCache = cache.GetCacheCatalog(new ConcreteCachedComposablePartCatalogSite());

            Assert.IsTrue(catalogCache.Parts.Count() == 0);
            Assert.IsTrue(catalogCache.GetExports( (ExportDefinition item) => item.ContractName == "FooExport").Count() == 0);
        }

        [TestMethod]
        public void VerifyReadSimplePart()
        {
            AssemblyCacheGenerator generator = AssemblyCacheGeneratorTests.BeginGeneration("Foo");
            ComposablePartDefinition expectedPartDefinition = PartDefinitionFactory.CreateAttributed(typeof(MyPart1));
            generator.CachePartDefinition(expectedPartDefinition);
            Type stubType = AssemblyCacheGeneratorTests.EndGeneration(generator);


            ComposablePartCatalogAssemblyCache cache = new ComposablePartCatalogAssemblyCache(stubType, new MockReader());
            ComposablePartCatalog catalogCache = cache.GetCacheCatalog(new ConcreteCachedComposablePartCatalogSite());

            Assert.IsTrue(catalogCache.Parts.Count() == 1);
            ComposablePartDefinition partDefinition = catalogCache.Parts.First();

            EnumerableAssert.AreEqual(expectedPartDefinition.Metadata, partDefinition.Metadata);

            Assert.AreEqual(expectedPartDefinition.ImportDefinitions.Count(), partDefinition.ImportDefinitions.Count());
            var expectedImportDefinition = (ContractBasedImportDefinition)expectedPartDefinition.ImportDefinitions.First();
            var importDefinition = (ContractBasedImportDefinition)partDefinition.ImportDefinitions.First();
            Assert.AreEqual(expectedImportDefinition.ContractName, importDefinition.ContractName);
            Assert.AreEqual(expectedImportDefinition.Cardinality, importDefinition.Cardinality);
            Assert.AreEqual(expectedImportDefinition.IsPrerequisite, importDefinition.IsPrerequisite);
            Assert.AreEqual(expectedImportDefinition.IsRecomposable, importDefinition.IsRecomposable);


            Assert.AreEqual(expectedPartDefinition.ExportDefinitions.Count(), partDefinition.ExportDefinitions.Count());
            ExportDefinition expectedExport = expectedPartDefinition.ExportDefinitions.First();
            ExportDefinition exportDefinition = partDefinition.ExportDefinitions.First();
            Assert.AreEqual(expectedExport.ContractName, exportDefinition.ContractName);
            EnumerableAssert.AreEqual(expectedExport.Metadata, exportDefinition.Metadata);
        }


        [TestMethod]
        public void VerifyParts()
        {
            AssemblyCacheGenerator generator = AssemblyCacheGeneratorTests.BeginGeneration("Foo");
            ComposablePartDefinition expectedPartDefinition1 = PartDefinitionFactory.CreateAttributed(typeof(MyPart1));
            ComposablePartDefinition expectedPartDefinition2 = PartDefinitionFactory.CreateAttributed(typeof(MyPart2));
            ComposablePartDefinition expectedPartDefinition3 = PartDefinitionFactory.CreateAttributed(typeof(MyPart3));
            generator.CachePartDefinition(expectedPartDefinition1);
            generator.CachePartDefinition(expectedPartDefinition2);
            generator.CachePartDefinition(expectedPartDefinition3);
            Type stubType = AssemblyCacheGeneratorTests.EndGeneration(generator);

            ComposablePartCatalogAssemblyCache cache = new ComposablePartCatalogAssemblyCache(stubType, new MockReader());
            ComposablePartCatalog catalogCache = cache.GetCacheCatalog(new ConcreteCachedComposablePartCatalogSite());

            List<ComposablePartDefinition> parts = catalogCache.Parts.ToList();
            Assert.AreEqual(3, parts.Count);
        }

        [TestMethod]
        public void PartsRepeatableRead()
        {
            AssemblyCacheGenerator generator = AssemblyCacheGeneratorTests.BeginGeneration("Foo");
            ComposablePartDefinition expectedPartDefinition1 = PartDefinitionFactory.CreateAttributed(typeof(MyPart1));
            ComposablePartDefinition expectedPartDefinition2 = PartDefinitionFactory.CreateAttributed(typeof(MyPart2));
            ComposablePartDefinition expectedPartDefinition3 = PartDefinitionFactory.CreateAttributed(typeof(MyPart3));
            generator.CachePartDefinition(expectedPartDefinition1);
            generator.CachePartDefinition(expectedPartDefinition2);
            generator.CachePartDefinition(expectedPartDefinition3);
            Type stubType = AssemblyCacheGeneratorTests.EndGeneration(generator);

            ComposablePartCatalogAssemblyCache cache = new ComposablePartCatalogAssemblyCache(stubType, new MockReader());
            ComposablePartCatalog catalogCache = cache.GetCacheCatalog(new ConcreteCachedComposablePartCatalogSite());

            List<ComposablePartDefinition> parts1 = catalogCache.Parts.ToList();
            List<ComposablePartDefinition> parts2 = catalogCache.Parts.ToList();

            Assert.IsTrue(parts1.SequenceEqual(parts2));
        }

        [TestMethod]
        public void GetExports()
        {
            var generator = AssemblyCacheGeneratorTests.BeginGeneration("Foo");
            var expectedPartDefinition1 = PartDefinitionFactory.CreateAttributed(typeof(MyPart1));
            var expectedPartDefinition2 = PartDefinitionFactory.CreateAttributed(typeof(MyPart2));
            var expectedPartDefinition3 = PartDefinitionFactory.CreateAttributed(typeof(MyPart3));
            generator.CachePartDefinition(expectedPartDefinition1);
            generator.CachePartDefinition(expectedPartDefinition2);
            generator.CachePartDefinition(expectedPartDefinition3);
            Type stubType = AssemblyCacheGeneratorTests.EndGeneration(generator);

            ComposablePartCatalogAssemblyCache cache = new ComposablePartCatalogAssemblyCache(stubType, new MockReader());
            ComposablePartCatalog catalogCache = cache.GetCacheCatalog(new ConcreteCachedComposablePartCatalogSite());

            List<Tuple<ComposablePartDefinition, ExportDefinition>> exports1 = catalogCache.GetExports((ExportDefinition item) => item.ContractName == "FooExport1").ToList();
            Assert.AreEqual(3, exports1.Count);

            List<Tuple<ComposablePartDefinition, ExportDefinition>> exports2 = catalogCache.GetExports((ExportDefinition item) => item.ContractName == "FooExport2").ToList();
            Assert.AreEqual(2, exports2.Count);

            List<Tuple<ComposablePartDefinition, ExportDefinition>> exports3 = catalogCache.GetExports((ExportDefinition item) => item.ContractName == "FooExport3").ToList();
            Assert.AreEqual(1, exports3.Count);

            List<Tuple<ComposablePartDefinition, ExportDefinition>> exports4 = catalogCache.GetExports((ExportDefinition item) => item.ContractName == "FooExport_NonExistent").ToList();
            Assert.AreEqual(0, exports4.Count);
        }

        [TestMethod]
        public void GetExportsRepeatableRead()
        {
            AssemblyCacheGenerator generator = AssemblyCacheGeneratorTests.BeginGeneration("Foo");
            ComposablePartDefinition expectedPartDefinition1 = PartDefinitionFactory.CreateAttributed(typeof(MyPart1));
            ComposablePartDefinition expectedPartDefinition2 = PartDefinitionFactory.CreateAttributed(typeof(MyPart2));
            ComposablePartDefinition expectedPartDefinition3 = PartDefinitionFactory.CreateAttributed(typeof(MyPart3));
            generator.CachePartDefinition(expectedPartDefinition1);
            generator.CachePartDefinition(expectedPartDefinition2);
            generator.CachePartDefinition(expectedPartDefinition3);
            Type stubType = AssemblyCacheGeneratorTests.EndGeneration(generator);

            ComposablePartCatalogAssemblyCache cache = new ComposablePartCatalogAssemblyCache(stubType, new MockReader());
            ComposablePartCatalog catalogCache = cache.GetCacheCatalog(new ConcreteCachedComposablePartCatalogSite());

            List<Tuple<ComposablePartDefinition, ExportDefinition>> exports1 = catalogCache.GetExports((ExportDefinition item) => item.ContractName == "FooExport1").ToList();
            List<Tuple<ComposablePartDefinition, ExportDefinition>> exports2 = catalogCache.GetExports((ExportDefinition item) => item.ContractName == "FooExport1").ToList();

            Assert.IsTrue(exports1.SequenceEqual(exports2));
        }

        [TestMethod]
        public void GetExportsByContractAndMetadata()
        {
            AssemblyCacheGenerator generator = AssemblyCacheGeneratorTests.BeginGeneration("Foo");
            ComposablePartDefinition expectedPartDefinition1 = PartDefinitionFactory.CreateAttributed(typeof(MyPart1));
            ComposablePartDefinition expectedPartDefinition2 = PartDefinitionFactory.CreateAttributed(typeof(MyPart2));
            ComposablePartDefinition expectedPartDefinition3 = PartDefinitionFactory.CreateAttributed(typeof(MyPart3));
            generator.CachePartDefinition(expectedPartDefinition1);
            generator.CachePartDefinition(expectedPartDefinition2);
            generator.CachePartDefinition(expectedPartDefinition3);
            Type stubType = AssemblyCacheGeneratorTests.EndGeneration(generator);

            ComposablePartCatalogAssemblyCache cache = new ComposablePartCatalogAssemblyCache(stubType, new MockReader());
            ComposablePartCatalog catalogCache = cache.GetCacheCatalog(new ConcreteCachedComposablePartCatalogSite());

            List<Tuple<ComposablePartDefinition, ExportDefinition>> exports = catalogCache.GetExports(ConstraintServices.CreateConstraint("FooExport1", null, new string[] { "Foo1" }, CreationPolicy.Any)).ToList();
            Assert.IsNotNull(exports);
            Assert.AreEqual(1, exports.Count());
        }
    }
}
