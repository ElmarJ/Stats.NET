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
using System.ComponentModel.Composition.Hosting;
using System.Collections;
using System.UnitTesting;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;

namespace System.ComponentModel.Composition.Caching.BinaryCaching
{
    [TestClass]
    public class BinaryCachingTests
    {
        [TestMethod]
        public void CachingRoundtrip()
        {
            AssemblyCatalog catalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());
            string cachePath = FileIO.GetTemporaryFileName("cache.bin");
            using (FileStream fs = new FileStream(cachePath, FileMode.Create, FileAccess.Write))
            {
                using (BinaryCacheWriter writer = new BinaryCacheWriter(fs))
                {
                    ((ICachedComposablePartCatalog)catalog).CacheCatalog(writer);
                }
            }

            AssemblyCatalog cachedCatalog;
            using (FileStream fs = new FileStream(cachePath, FileMode.Open, FileAccess.Read))
            {
                using (BinaryCacheReader reader = new BinaryCacheReader(fs))
                {
                    cachedCatalog = (AssemblyCatalog)reader.ReadRootCatalog();
                }
            }

            Assert.AreEqual(catalog.Parts.Count(), cachedCatalog.Parts.Count());
        }
    }
}
