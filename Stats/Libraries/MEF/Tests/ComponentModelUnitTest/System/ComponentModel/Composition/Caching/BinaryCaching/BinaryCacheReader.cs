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
using System.UnitTesting;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;

namespace System.ComponentModel.Composition.Caching.BinaryCaching
{
    internal class BinaryCacheReader : ComposablePartCatalogCacheReader
    {
        private CacheRecord _cacheRecord;

        public BinaryCacheReader(Stream stream)
        {
            using (GZipStream compressedStream = new GZipStream(stream, CompressionMode.Decompress, true))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                this._cacheRecord = (CacheRecord)formatter.Deserialize(compressedStream);
            }
        }


        protected override ComposablePartCatalogCache ReadCacheCore(object cacheToken)
        {
            return new BinaryCache(this, (CatalogRecord)cacheToken);
        }

        protected override object RootCacheToken
        {
            get
            {
                return this._cacheRecord.RootCatalog;
            }
        }
    }
}
