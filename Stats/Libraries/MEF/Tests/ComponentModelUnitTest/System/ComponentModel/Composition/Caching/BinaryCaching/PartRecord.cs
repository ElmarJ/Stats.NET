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
    internal class PartRecord
    {
        public IDictionary<string, object> PartCache { get; set; }
        public IDictionary<string, object>[] ExportsCache { get; set; }
        public IDictionary<string, object>[] ImportsCache { get; set; }
    }
}
