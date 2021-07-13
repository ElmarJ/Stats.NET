// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.AttributedModel;
using System.ComponentModel.Composition.Factories;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection.Emit;
using System.Reflection;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Caching;
using System.Collections;
using System.IO;
using System.UnitTesting;

namespace System.ComponentModel.Composition.Caching
{
    [TestClass]
    public class ComposablePartCatalogAssemblyCacheWriterTests
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

        internal class MyCatalog : ComposablePartCatalog
        {
            public MyCatalog() { }
            internal MyCatalog(ComposablePartCatalogCache cache)
            {
            }

            public override IQueryable<ComposablePartDefinition> Parts
            {
                get { throw new NotImplementedException(); }
            }

            protected override void Dispose(bool disposing)
            {
                throw new NotImplementedException();
            }
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

        private class NoncachedCatalog : ComposablePartCatalog
        {
            public override IQueryable<ComposablePartDefinition> Parts
            {
                get { throw new NotImplementedException(); }
            }
        }


        private static ComposablePartCatalogAssemblyCache GetAssemblyCache(string cachePath, string catalogIdentifier)
        {
            AssemblyName assemblyName = new AssemblyName(Path.GetFileNameWithoutExtension(cachePath));
            assemblyName.CodeBase = cachePath;
            Assembly assembly = AppDomain.CurrentDomain.Load(assemblyName);
            Type stubType = assembly.GetType(CacheStructureConstants.CachingStubTypeNamePrefix + catalogIdentifier);
            Assert.IsNotNull(stubType);
            return new ComposablePartCatalogAssemblyCache(stubType, new MockReader());

        }

        private static Type GetRootStubType(ComposablePartCatalogAssemblyCache cache)
        {
            Type entryPointType = cache.StubType.Assembly.GetType(CacheStructureConstants.EntryPointTypeName);
            Assert.IsNotNull(entryPointType);
            MethodInfo getRootStubMethod = entryPointType.GetMethod(CacheStructureConstants.EntryPointGetRootStubMethodName, BindingFlags.Static | BindingFlags.Public);
            Assert.IsNotNull(getRootStubMethod);
            Func<string> getRootStub = (Func<string>)Delegate.CreateDelegate(typeof(Func<string>), getRootStubMethod);
            string stubSuffix = getRootStub.Invoke();
            return cache.StubType.Assembly.GetType(CacheStructureConstants.CachingStubTypeNamePrefix + stubSuffix);
        }

        [TestMethod]
        [Ignore]
        [WorkItem(507696)]
        public void WriteSimpleCatalogWithFullPath()
        {
            string cachePath = FileIO.GetTemporaryFileName("cache.dll");
            Assert.IsFalse(File.Exists(cachePath));
            ComposablePartDefinition expectedPartDefinition1 = PartDefinitionFactory.CreateAttributed(typeof(MyPart1));
            ComposablePartDefinition expectedPartDefinition2 = PartDefinitionFactory.CreateAttributed(typeof(MyPart2));
            ComposablePartDefinition expectedPartDefinition3 = PartDefinitionFactory.CreateAttributed(typeof(MyPart3));

            IDictionary<string, object> expectedMetadata = new Dictionary<string, object>();
            expectedMetadata.Add("A", 42);
            expectedMetadata.Add("B", "Foo");
            expectedMetadata.Add("C", BindingFlags.ExactBinding);
            expectedMetadata.Add("D", this.GetType());

            List<ComposablePartDefinition> partDefinitions = new List<ComposablePartDefinition>() {expectedPartDefinition1, expectedPartDefinition2, expectedPartDefinition3};
            using (ComposablePartCatalogCacheWriter writer = this.CreateAssemblyCacheWriter(cachePath))
            {
                object catalogToken = writer.WriteCache(typeof(MyCatalog), partDefinitions, expectedMetadata, new ConcreteCachedComposablePartCatalogSite());
                writer.WriteRootCacheToken(catalogToken);
                Assert.IsTrue(catalogToken is string);
            }


            Assert.IsTrue(File.Exists(cachePath));
            ComposablePartCatalogAssemblyCache cache = GetAssemblyCache(cachePath, "0");

            expectedMetadata.Add(ComposablePartCatalogCacheReader.CatalogTypeMetadataKey, typeof(MyCatalog));
            EnumerableAssert.AreEqual(expectedMetadata, cache.Metadata);

            Assert.AreEqual(3, cache.GetCacheCatalog(new ConcreteCachedComposablePartCatalogSite()).Parts.Count());

            Type rootStubType = GetRootStubType(cache);
            Assert.IsNotNull(rootStubType);

            ComposablePartCatalogAssemblyCache rootCache = new ComposablePartCatalogAssemblyCache(rootStubType, new MockReader());
            Assert.AreEqual(cache.GetCacheCatalog(new ConcreteCachedComposablePartCatalogSite()).Parts.Count(), rootCache.GetCacheCatalog(new ConcreteCachedComposablePartCatalogSite()).Parts.Count());
        }

        [TestMethod]
        [Ignore]
        [WorkItem(507696)]
        public void WriteSimpleCatalogWithPartialPath()
        {
            string cachePath = FileIO.GetTemporaryFileName("cache.dll");
            Assert.IsFalse(File.Exists(cachePath));
            ComposablePartDefinition expectedPartDefinition1 = PartDefinitionFactory.CreateAttributed(typeof(MyPart1));
            ComposablePartDefinition expectedPartDefinition2 = PartDefinitionFactory.CreateAttributed(typeof(MyPart2));
            ComposablePartDefinition expectedPartDefinition3 = PartDefinitionFactory.CreateAttributed(typeof(MyPart3));

            IDictionary<string, object> expectedMetadata = new Dictionary<string, object>();
            expectedMetadata.Add("A", 42);
            expectedMetadata.Add("B", "Foo");
            expectedMetadata.Add("C", BindingFlags.ExactBinding);
            expectedMetadata.Add("D", this.GetType());

            List<ComposablePartDefinition> partDefinitions = new List<ComposablePartDefinition>() { expectedPartDefinition1, expectedPartDefinition2, expectedPartDefinition3 };
            using (ComposablePartCatalogCacheWriter writer = this.CreateAssemblyCacheWriter(cachePath))
            {
                object catalogToken = writer.WriteCache(typeof(MyCatalog), partDefinitions, expectedMetadata, new ConcreteCachedComposablePartCatalogSite());
                Assert.IsTrue(catalogToken is string);
            }

            Assert.IsTrue(File.Exists(cachePath));
            ComposablePartCatalogAssemblyCache cache = GetAssemblyCache(cachePath, "0");

            expectedMetadata.Add(ComposablePartCatalogCacheReader.CatalogTypeMetadataKey, typeof(MyCatalog));
            EnumerableAssert.AreEqual(expectedMetadata, cache.Metadata);

            Assert.AreEqual(3, cache.GetCacheCatalog(new ConcreteCachedComposablePartCatalogSite()).Parts.Count());

            Type rootStubType = GetRootStubType(cache);
            Assert.IsNull(rootStubType);
        }

        [TestMethod]
        public void WriteMultipleCatalogsIntoCache()
        {
            string cachePath = FileIO.GetTemporaryFileName("Many.cache.dll");
            Assert.IsFalse(File.Exists(cachePath));
            ComposablePartDefinition expectedPartDefinition1 = PartDefinitionFactory.CreateAttributed(typeof(MyPart1));
            ComposablePartDefinition expectedPartDefinition2 = PartDefinitionFactory.CreateAttributed(typeof(MyPart2));
            ComposablePartDefinition expectedPartDefinition3 = PartDefinitionFactory.CreateAttributed(typeof(MyPart3));


            List<ComposablePartDefinition> partDefinitions = new List<ComposablePartDefinition>() { expectedPartDefinition1, expectedPartDefinition2, expectedPartDefinition3 };
            using (ComposablePartCatalogCacheWriter writer = this.CreateAssemblyCacheWriter(cachePath))
            {
                for (int i = 0; i < 20; i++)
                {
                    object catalogToken = writer.WriteCache(typeof(MyCatalog), partDefinitions, null, new ConcreteCachedComposablePartCatalogSite());
                    Assert.IsTrue(catalogToken is string);
                }
            }

            Assert.IsTrue(File.Exists(cachePath));

            for (int i = 0; i < 20; i++)
            {
                ComposablePartCatalogAssemblyCache cache = GetAssemblyCache(cachePath, "0");
                Assert.AreEqual(3, cache.GetCacheCatalog(new ConcreteCachedComposablePartCatalogSite()).Parts.Count());
            }
        }

        [TestMethod]
        public void WriteMultipleCatalogsIntoOneCache()
        {
            string cachePath = FileIO.GetTemporaryFileName("One.cache.dll");
            Assert.IsFalse(File.Exists(cachePath));
            ComposablePartDefinition expectedPartDefinition1 = PartDefinitionFactory.CreateAttributed(typeof(MyPart1));
            ComposablePartDefinition expectedPartDefinition2 = PartDefinitionFactory.CreateAttributed(typeof(MyPart2));
            ComposablePartDefinition expectedPartDefinition3 = PartDefinitionFactory.CreateAttributed(typeof(MyPart3));


            List<ComposablePartDefinition> partDefinitions = new List<ComposablePartDefinition>();
            for (int i = 0; i < 20; i++)
            {
                partDefinitions.Add(expectedPartDefinition1);
                partDefinitions.Add(expectedPartDefinition2);
                partDefinitions.Add(expectedPartDefinition3);
            }
            using (ComposablePartCatalogCacheWriter writer = this.CreateAssemblyCacheWriter(cachePath))
            {
                object catalogToken = writer.WriteCache(typeof(MyCatalog), partDefinitions, null, new ConcreteCachedComposablePartCatalogSite());
                Assert.IsTrue(catalogToken is string);
            }

            Assert.IsTrue(File.Exists(cachePath));
            ComposablePartCatalogAssemblyCache cache = GetAssemblyCache(cachePath, "0");
            Assert.AreEqual(60, cache.GetCacheCatalog(new ConcreteCachedComposablePartCatalogSite()).Parts.Count());
        }

        [TestMethod]
        public void WriteThisCatalogOneCache()
        {
            string cachePath = FileIO.GetTemporaryFileName("This.cache.dll");
            Assert.IsFalse(File.Exists(cachePath));
            ComposablePartCatalog catalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());
            using (ComposablePartCatalogCacheWriter writer = this.CreateAssemblyCacheWriter(cachePath))
            {
                object catalogToken = writer.WriteCache(typeof(MyCatalog), catalog.Parts, null, new ConcreteCachedComposablePartCatalogSite());
                Assert.IsTrue(catalogToken is string);
            }

            Assert.IsTrue(File.Exists(cachePath));
            ComposablePartCatalogAssemblyCache cache = GetAssemblyCache(cachePath, "0");
            Assert.AreEqual(catalog.Parts.Count(), cache.GetCacheCatalog(new ConcreteCachedComposablePartCatalogSite()).Parts.Count());
        }


        [TestMethod]
        public void WriteAfterDispose()
        {
            string cachePath = FileIO.GetTemporaryFileName("cache.dll");
            ComposablePartCatalogCacheWriter writer = this.CreateAssemblyCacheWriter(cachePath);
            writer.Dispose();

            ExceptionAssert.ThrowsDisposed(writer,  () =>
            {
                writer.WriteCache(typeof(MyCatalog), null, null, new ConcreteCachedComposablePartCatalogSite());
            });
        }

        [TestMethod]
        public void WriteNonCacheableCatalog()
        {
            string cachePath = FileIO.GetTemporaryFileName("cache.dll");
            ComposablePartCatalogCacheWriter writer = this.CreateAssemblyCacheWriter(cachePath);

            ExceptionAssert.Throws<InvalidOperationException>( () =>
            {
                writer.WriteCache(typeof(NoncachedCatalog), null, null, new ConcreteCachedComposablePartCatalogSite());
            });

        }

        private ComposablePartCatalogAssemblyCacheWriter CreateAssemblyCacheWriter(string cachePath)
        {
            AssemblyName name = new AssemblyName(Path.GetFileNameWithoutExtension(cachePath));
            return new ComposablePartCatalogAssemblyCacheWriter(name, Path.GetDirectoryName(Path.GetFullPath(cachePath)));
        }
    }
}
