// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
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
using System.Linq.Expressions;
using System.UnitTesting;
using System.ComponentModel.Composition.AttributedModel;
using System.ComponentModel.Composition.Factories;

namespace System.ComponentModel.Composition.Caching
{
    [TestClass]
    public class ComposablePartCatalogAssemblyCacheReaderTests
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

        internal class MyCatalog : ComposablePartCatalog, ICachedComposablePartCatalog
        {
            public IList<ComposablePartDefinition> InnerParts;
            private ComposablePartCatalogCache _cache;
            private ComposablePartCatalog _catalogCache;

            public MyCatalog()
            {
                this.InnerParts = new List<ComposablePartDefinition>(); ;
            }

            internal MyCatalog(ComposablePartCatalogCache cache)
            {
                this._cache = cache; 
                this._catalogCache = this._cache.GetCacheCatalog(new ConcreteCachedComposablePartCatalogSite());
            }

            public override IQueryable<ComposablePartDefinition> Parts
            {
                get
                {
                    if (this._cache == null)
                    {
                        return this.InnerParts.AsQueryable();
                    }
                    else
                    {
                        return this._catalogCache.Parts;
                    }
                }
            }

            public override IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> GetExports(ImportDefinition definition)
            {
                if (this._cache == null)
                {
                    return base.GetExports(definition);
                }
                else
                {
                    return this._catalogCache.GetExports(definition);
                }
            }

            object ICachedComposablePartCatalog.CacheCatalog(ComposablePartCatalogCacheWriter cacheWriter)
            {
                object token = cacheWriter.WriteCache(this.GetType(), this.InnerParts, null, new ConcreteCachedComposablePartCatalogSite());
                cacheWriter.WriteRootCacheToken(token);
                return token;
            }

            bool ICachedComposablePartCatalog.IsCacheUpToDate
            {
                get { throw new NotImplementedException(); }
            }

        }

        [TestMethod]
        public void CacheSimpleCatalog()
        {
            MyCatalog catalog = new MyCatalog();
            ComposablePartDefinition expectedPartDefinition1 = PartDefinitionFactory.CreateAttributed(typeof(MyPart1));
            ComposablePartDefinition expectedPartDefinition2 = PartDefinitionFactory.CreateAttributed(typeof(MyPart2));
            ComposablePartDefinition expectedPartDefinition3 = PartDefinitionFactory.CreateAttributed(typeof(MyPart3));
            catalog.InnerParts.Add(expectedPartDefinition1);
            catalog.InnerParts.Add(expectedPartDefinition2);
            catalog.InnerParts.Add(expectedPartDefinition3);

            MyCatalog catalog2 = (MyCatalog)catalog.GetCachedCatalog();

            Assert.AreEqual(catalog.Parts.Count(), catalog2.Parts.Count());
        }
    }
}
