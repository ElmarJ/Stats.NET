// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;

namespace System.ComponentModel.Composition.Caching.AttributedModel
{
    [Export]
    [Export(typeof(Part))]
    public class Part
    {
        [ImportingConstructor]
        public Part([Import(AllowDefault = true)]string p1,
                    [Import(AllowDefault = false)]string p2,
                    [ImportMany]IEnumerable<string> p3,
                    [ImportMany]IEnumerable<string> p4)
        {
        }

        [Import(AllowDefault = true)]
        public string FieldImport1;

        [Import(AllowDefault = false)]
        public string FieldImport2;

        [ImportMany]
        public IEnumerable<string> FieldImport3;

        [Import(AllowDefault = true)]
        public string PropertyImport1 { get; set; }

        [Import(AllowDefault = true)]
        public string PropertyImport2 { set { } }

        [Import(AllowDefault = false)]
        public string PropertyImport3 { get { return null; } }

        [Import(AllowDefault = false)]
        public string PropertyImport4 { get; set; }

        [ImportMany]
        public IEnumerable<string> PropertyImport5 { set { } }

        [ImportMany]
        public IEnumerable<string> PropertyImport6 { get { return null; } }

        [ImportMany]
        public IEnumerable<string> PropertyImport7 { get; set; }

        [Export]
        public string FieldExport1;

        [Export]
        public string PropertyExport1 { get { return null; } }

        [Export]
        public string PropertyExport2 { get { return null; } set { } }

        [Export]
        public string PropertyExport3 { set { } }
    }

    [Export]
    [Export(typeof(Part))]
    public class DerivedPart : Part
    {
        [ImportingConstructor]
        public DerivedPart([Import(AllowDefault = true)]string p1,
                           [Import(AllowDefault = false)]string p2,
                           [ImportMany]IEnumerable<string> p3,
                           [ImportMany]IEnumerable<string> p4)
                           : base(p1, p2, p3, p4)
        {
        }

        [Import(AllowDefault = true)]
        public new string FieldImport1;

        [Import(AllowDefault = false)]
        public new string FieldImport2;

        [ImportMany]
        public new IEnumerable<string> FieldImport3;

        [Import(AllowDefault = true)]
        public new string PropertyImport1 { get; set; }

        [Import(AllowDefault = true)]
        public new string PropertyImport2 { set { } }

        [Import(AllowDefault = false)]
        public new string PropertyImport3 { get { return null; } }

        [Import(AllowDefault = false)]
        public new string PropertyImport4 { get; set; }

        [ImportMany]
        public new IEnumerable<string> PropertyImport5 { set { } }

        [ImportMany]
        public new IEnumerable<string> PropertyImport6 { get { return null; } }

        [ImportMany]
        public new IEnumerable<string> PropertyImport7 { get; set; }

        [Export]
        public new string FieldExport1;

        [Export]
        public new string PropertyExport1 { get { return null; } }

        [Export]
        public new string PropertyExport2 { get { return null; } set { } }

        [Export]
        public new string PropertyExport3 { set { } }
    }

    public static class StaticPart
    {
        [Export]
        public static string FieldExport1;

        [Export]
        public static string PropertyExport1 { get { return null; } }

        [Export]
        public static string PropertyExport2 { get { return null; } set { } }

        [Export]
        public static string PropertyExport3 { set { } }
    }
}
