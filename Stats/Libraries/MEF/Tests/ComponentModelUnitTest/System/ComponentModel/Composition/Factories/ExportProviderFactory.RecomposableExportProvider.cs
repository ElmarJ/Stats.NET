// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.Composition.Factories
{
    partial class ExportProviderFactory
    {
        public class RecomposableExportProvider : ExportProvider
        {
            public List<Export> _exports = new List<Export>();

            public void AddExport(string contractName, object value)
            {
                Export export = new Export(contractName, () => value);
                _exports.Add(export);
            }

            public void ReplaceExportValue(string contractName, object newValue)
            {
                int index = FindExport(contractName);

                Assert.IsTrue(index >= 0);

                _exports[index] = new Export(contractName, () => newValue);
                FireExportsChangedEvent(contractName);
            }

            private int FindExport(string contractName)
            {
                for (int i = 0; i < _exports.Count; i++)
                {
                    if (_exports[i].Definition.ContractName == contractName)
                    {
                        return i;
                    }
                }
                return -1;
            }

            protected override IEnumerable<Export> GetExportsCore(ImportDefinition importDefinition)
            {
                List<Export> exports = new List<Export>();
                var func = importDefinition.Constraint.Compile();
                foreach (Export export in _exports)
                {
                    if (func(export.Definition))
                    {
                        exports.Add(export);
                    }
                }
                return exports;
            }

            private void FireExportsChangedEvent(params string[] changedNames)
            {
                OnExportsChanged(new ExportsChangedEventArgs(changedNames));
            }
        }
    }

}