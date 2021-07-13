// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.ComponentModel.Composition.Factories;
using System.ComponentModel.Composition.Primitives;

namespace System.ComponentModel.Composition
{
    internal class ConcreteComposablePart : ComposablePart
    {
        private readonly List<Export> _exports = new List<Export>();
        private readonly List<ImportDefinition> _imports = new List<ImportDefinition>();
        private readonly IDictionary<string, IEnumerable<Export>> _setImports = new Dictionary<string, IEnumerable<Export>>();

        public ConcreteComposablePart()
        {
        }

        public override IDictionary<string, object> Metadata
        {
            get { return MetadataServices.EmptyMetadata; }
        }

        public IDictionary<string, IEnumerable<Export>> SetImports
        {
            get { return this._setImports; }
        }

        public override IEnumerable<ExportDefinition> ExportDefinitions
        {
            get { return this._exports.Select(export => export.Definition); }
        }

        public override IEnumerable<ImportDefinition> ImportDefinitions
        {
            get { return this._imports; }
        }

        public ImportDefinition AddImport(string contractName, ImportCardinality cardinality, bool isRecomposable, bool isPrerequisite)
        {
            var import = ImportDefinitionFactory.CreateDefault(contractName, cardinality, isRecomposable, isPrerequisite);

            this.AddImport(import);
            return import;
        }

        public void AddImport(ImportDefinition import)
        {
            this._imports.Add(import);
        }

        public void AddExport(string contractName, object value)
        {
            this.AddExport(ExportFactory.Create(contractName, () => value));
        }

        public void AddExport(Export export)
        {
            this._exports.Add(export);
        }

        public override object GetExportedObject(ExportDefinition definition)
        {
            Export export = _exports.First(e => e.Definition == definition);

            return export.GetExportedObject();
        }

        public override void SetImport(ImportDefinition definition, IEnumerable<Export> exports)
        {
            ContractBasedImportDefinition contractBasedDefinition = (ContractBasedImportDefinition)definition;
            this._setImports[contractBasedDefinition.ContractName] = exports;

            foreach (Export export in exports)
            {
                export.GetExportedObject();
            }
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    foreach (var disposable in _exports.Select(export => export.GetExportedObject()).OfType<IDisposable>())
                    {
                        disposable.Dispose();
                    }
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }
    }
}

