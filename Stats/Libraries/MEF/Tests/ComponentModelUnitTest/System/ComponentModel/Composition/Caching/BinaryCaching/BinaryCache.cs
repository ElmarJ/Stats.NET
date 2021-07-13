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
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;

namespace System.ComponentModel.Composition.Caching.BinaryCaching
{
    internal class BinaryCache : ComposablePartCatalogCache
    {
        private BinaryCacheReader _reader;
        private CatalogRecord _catalogRecord;

        public BinaryCache(BinaryCacheReader reader, CatalogRecord catalogRecord)
        {
            this._catalogRecord = catalogRecord;
            this._reader = reader;
        }

        public override IDictionary<string, object> Metadata
        {
            get
            {
                return this._catalogRecord.Metadata;
            }
        }

        public override ComposablePartCatalog GetCacheCatalog(ICachedComposablePartCatalogSite catalogSite)
        {
            return new BinaryCacheCatalog(this._catalogRecord, catalogSite);
        }

        public override ComposablePartCatalogCacheReader Reader
        {
            get
            {
                return this._reader;
            }
        }
    }
}
