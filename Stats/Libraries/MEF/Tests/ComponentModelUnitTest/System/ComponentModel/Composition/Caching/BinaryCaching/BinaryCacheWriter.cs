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
using System.ComponentModel.Composition.Primitives;

namespace System.ComponentModel.Composition.Caching.BinaryCaching
{
    internal class BinaryCacheWriter : ComposablePartCatalogCacheWriter
    {
        private CacheRecord _cacheRecord;
        private Stream _stream;

        public BinaryCacheWriter(Stream stream)
        {
            this._stream = stream;
            this._cacheRecord = new CacheRecord();
        }

        protected override object WriteCacheCore(IEnumerable<ComposablePartDefinition> partDefinitions, IDictionary<string, object> catalogMetadata, ICachedComposablePartCatalogSite catalogSite)
        {
            CatalogRecord catalogRecord = new CatalogRecord();
            catalogRecord.Metadata = this.ProcessDictionary(catalogMetadata);
            catalogRecord.Parts = new PartRecord[partDefinitions.Count()];

            int i = 0;
            foreach (ComposablePartDefinition partDefinition in partDefinitions)
            {
                PartRecord partRecord = new PartRecord();
                partRecord.PartCache = this.ProcessDictionary(catalogSite.CachePartDefinition(partDefinition));

                partRecord.ExportsCache = partDefinition.ExportDefinitions
                    .Select(export => this.ProcessDictionary(catalogSite.CacheExportDefinition(partDefinition, export)))
                    .ToArray();

                partRecord.ImportsCache = partDefinition.ImportDefinitions
                    .Select(import => this.ProcessDictionary(catalogSite.CacheImportDefinition(partDefinition, import)))
                    .ToArray();
                catalogRecord.Parts[i] = partRecord;
                i++;
            }
            this._cacheRecord.Catalogs.Add(catalogRecord);

            return catalogRecord;
        }

        public override void WriteRootCacheToken(object cacheToken)
        {
            this._cacheRecord.RootCatalog = (CatalogRecord)cacheToken;
        }

        protected override void Dispose(bool disposing)
        {
            using (GZipStream compressedStream = new GZipStream(this._stream, CompressionMode.Compress, true))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(compressedStream, this._cacheRecord);
            }

            base.Dispose(disposing);
        }

        private IDictionary<string, object> ProcessDictionary(IDictionary<string, object> dictionary)
        {
            Dictionary<string, object> newDictionary = new Dictionary<string, object>();
            foreach (KeyValuePair<string, object> kvp in dictionary)
            {
                object value = kvp.Value;
                if (value is IDictionary<string, object>)
                {
                    value = this.ProcessDictionary((IDictionary<string, object>)value);
                }
                else if (value is IEnumerable<string>)
                {
                    value = ((IEnumerable<string>)value).ToArray();
                }
                else if (value is IEnumerable<long>)
                {
                    value = ((IEnumerable<long>)value).ToArray();
                }
                else if (value is IEnumerable<object>)
                {
                    value = ((IEnumerable<object>)value).ToArray();
                }

                newDictionary.Add(kvp.Key, value);
            }
            return newDictionary;
        }

    }
}
