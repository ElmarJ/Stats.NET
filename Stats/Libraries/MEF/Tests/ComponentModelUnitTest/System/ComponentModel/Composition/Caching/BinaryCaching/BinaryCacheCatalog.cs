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
using System.UnitTesting;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;

namespace System.ComponentModel.Composition.Caching.BinaryCaching
{
    internal class BinaryCacheCatalog : ComposablePartCatalog
    {
        private CatalogRecord _catalogRecord;
        private ICachedComposablePartCatalogSite _catalogSite;
        private IEnumerable<ComposablePartDefinition> _parts;

        public BinaryCacheCatalog(CatalogRecord catalogRecord, ICachedComposablePartCatalogSite catalogSite)
        {
            this._catalogRecord = catalogRecord;
            this._catalogSite = catalogSite;
        }

        public override IQueryable<ComposablePartDefinition> Parts
        {
            get
            {
                if (this._parts == null)
                {
                    List<ComposablePartDefinition> parts = new List<ComposablePartDefinition>();
                    foreach (PartRecord partRecord in this._catalogRecord.Parts)
                    {
                        ComposablePartDefinition part = this._catalogSite.CreatePartDefinitionFromCache(
                             partRecord.PartCache,
                             (thisPart) => partRecord.ImportsCache.Select(importCache => this._catalogSite.CreateImportDefinitionFromCache(thisPart, importCache)).ToArray(),
                             (thisPart) => partRecord.ExportsCache.Select(exportCache => this._catalogSite.CreateExportDefinitionFromCache(thisPart, exportCache)).ToArray());

                        parts.Add(part);
                    }
                    this._parts = parts;
                }
                return this._parts.AsQueryable();
            }
        }
    }
}
