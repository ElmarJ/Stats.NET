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

namespace System.ComponentModel.Composition.Caching.BinaryCaching
{
    [Serializable]
    internal class CatalogRecord
    {
        public PartRecord[] Parts { get; set; }
        public IDictionary<string, object> Metadata { get; set; }
    }

}
