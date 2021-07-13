// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Primitives;

namespace System.ComponentModel.Composition.Factories
{
    partial class ExportFactory
    {
        private class DerivedExport<T, TMetadataView> : Export<T, TMetadataView>
        {
            private readonly ExportDefinition _definition;
            private readonly Func<T> _exportedObjectGetter;

            public DerivedExport(ExportDefinition definition, Func<T> exportedObjectGetter)
            {
                _definition = definition;
                _exportedObjectGetter = exportedObjectGetter;
            }

            public override ExportDefinition Definition
            {
                get { return _definition; }
            }

            public override TMetadataView MetadataView
            {
                get { throw new NotImplementedException(); }
            }

            protected override object GetExportedObjectCore()
            {
                return _exportedObjectGetter();
            }
        }
    }
}
