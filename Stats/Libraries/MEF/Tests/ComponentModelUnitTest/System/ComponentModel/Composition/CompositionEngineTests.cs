// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Factories;
using System.ComponentModel.Composition.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.Composition
{
    [TestClass]
    public class CompositionEngineTests
    {
        //TODO: Now that CompositionEngine is public we need to add a complete set of unit tests for it.

        [TestMethod]
        public void Import_NonRecomposable_ValueShouldNotChange()
        {
            var engine = new CompositionEngine();
            var exportProvider = ExportProviderFactory.CreateRecomposable();
            engine.SourceProvider = exportProvider;

            exportProvider.AddExport("Value", 21);

            var import = ImportDefinitionFactory.Create("Value", false);
            var importer = PartFactory.CreateImporter(import);

            engine.SatisfyImports(importer, true);

            Assert.AreEqual(21, importer.GetImport(import));

            exportProvider.ReplaceExportValue("Value", 42);

            Assert.AreEqual(21, importer.GetImport(import), "Value should not change!");
        }

        [TestMethod]
        public void Import_Recomposable_ValueShouldChange()
        {
            var engine = new CompositionEngine();
            var exportProvider = ExportProviderFactory.CreateRecomposable();
            engine.SourceProvider = exportProvider;

            exportProvider.AddExport("Value", 21);

            var import = ImportDefinitionFactory.Create("Value", true);
            var importer = PartFactory.CreateImporter(import);

            engine.SatisfyImports(importer, true);

            Assert.AreEqual(21, importer.GetImport(import));

            exportProvider.ReplaceExportValue("Value", 42);

            Assert.AreEqual(42, importer.GetImport(import), "Value should change!");
        }

        [TestMethod]
        public void Import_Recomposable_ValueShouldNotChange_NoRecompositionRequested()
        {
            var engine = new CompositionEngine();
            var exportProvider = ExportProviderFactory.CreateRecomposable();
            engine.SourceProvider = exportProvider;

            exportProvider.AddExport("Value", 21);

            var import = ImportDefinitionFactory.Create("Value", true);
            var importer = PartFactory.CreateImporter(import);

            engine.SatisfyImports(importer, false);

            Assert.AreEqual(21, importer.GetImport(import));

            exportProvider.ReplaceExportValue("Value", 42);

            Assert.AreEqual(21, importer.GetImport(import), "Value should not change!");
        }

        [TestMethod]
        public void Import_Recomposable_ValueShouldNotChange_NoRecompositionRequested_ViaNonArgumentSignature()
        {
            var engine = new CompositionEngine();
            var exportProvider = ExportProviderFactory.CreateRecomposable();
            engine.SourceProvider = exportProvider;

            exportProvider.AddExport("Value", 21);

            var import = ImportDefinitionFactory.Create("Value", true);
            var importer = PartFactory.CreateImporter(import);

            engine.SatisfyImports(importer);

            Assert.AreEqual(21, importer.GetImport(import));

            exportProvider.ReplaceExportValue("Value", 42);

            Assert.AreEqual(21, importer.GetImport(import), "Value should not change!");
        }


        [TestMethod]
        public void Import_NonRecomposable_Prerequisite_ValueShouldNotChange()
        {
            var engine = new CompositionEngine();
            var exportProvider = ExportProviderFactory.CreateRecomposable();
            engine.SourceProvider = exportProvider;

            var import = ImportDefinitionFactory.Create("Value", false, true);
            var importer = PartFactory.CreateImporter(import);

            exportProvider.AddExport("Value", 21);

            engine.SatisfyImports(importer, true);

            Assert.AreEqual(21, importer.GetImport(import));

            exportProvider.ReplaceExportValue("Value", 42);

            Assert.AreEqual(21, importer.GetImport(import), "Value should NOT change!");
        }

        [TestMethod]
        public void Import_Recomposable_Prerequisite_ValueShouldChange()
        {
            var engine = new CompositionEngine();
            var exportProvider = ExportProviderFactory.CreateRecomposable();
            engine.SourceProvider = exportProvider;

            var import = ImportDefinitionFactory.Create("Value", true, true);
            var importer = PartFactory.CreateImporter(import);

            exportProvider.AddExport("Value", 21);

            engine.SatisfyImports(importer, true);

            Assert.AreEqual(21, importer.GetImport(import));

            exportProvider.ReplaceExportValue("Value", 42);

            Assert.AreEqual(42, importer.GetImport(import), "Value should change!");
        }

        [TestMethod]
        public void Import_OneRecomposable_OneNotRecomposable()
        {
            var engine = new CompositionEngine();
            var exportProvider = ExportProviderFactory.CreateRecomposable();
            engine.SourceProvider = exportProvider;

            var import1 = ImportDefinitionFactory.Create("Value", true);
            var import2 = ImportDefinitionFactory.Create("Value", false);
            var importer = PartFactory.CreateImporter(import1, import2);

            exportProvider.AddExport("Value", 21);

            engine.SatisfyImports(importer, true);

            // Initial compose values should be 21
            Assert.AreEqual(21, importer.GetImport(import1));
            Assert.AreEqual(21, importer.GetImport(import2));

            // Reset value to ensure it doesn't get set to same value again
            importer.ResetImport(import1);
            importer.ResetImport(import2);

            exportProvider.ReplaceExportValue("Value", 42);

            Assert.AreEqual(42, importer.GetImport(import1), "Value should have been set!");
            Assert.AreEqual(null, importer.GetImport(import2), "Value should NOT been set!");
        }

        [TestMethod]
        public void Import_TwoRecomposables_SingleExportValueChanged()
        {
            var engine = new CompositionEngine();
            var exportProvider = ExportProviderFactory.CreateRecomposable();
            engine.SourceProvider = exportProvider;

            var import1 = ImportDefinitionFactory.Create("Value1", true);
            var import2 = ImportDefinitionFactory.Create("Value2", true);
            var importer = PartFactory.CreateImporter(import1, import2);

            exportProvider.AddExport("Value1", 21);
            exportProvider.AddExport("Value2", 23);

            engine.SatisfyImports(importer, true);

            Assert.AreEqual(21, importer.GetImport(import1));
            Assert.AreEqual(23, importer.GetImport(import2));

            importer.ResetImport(import1);
            importer.ResetImport(import2);

            // Only change Value1 
            exportProvider.ReplaceExportValue("Value1", 42);

            Assert.AreEqual(42, importer.GetImport(import1), "Value should have been set!");

            // Currently we recompose all recomposable imports on a part when any of the
            // imports on that part get recomposed. In the future we may consider actually only
            // composing changed imports but the current composition engine doesn't support it.
            Assert.AreEqual(23, importer.GetImport(import2), "Value should have changed to the value in the container.");
        }

        [TestMethod]
        public void Import_Recomposable_Unregister_ValueShouldChangeOnce()
        {
            var engine = new CompositionEngine();
            var exportProvider = ExportProviderFactory.CreateRecomposable();
            engine.SourceProvider = exportProvider;

            exportProvider.AddExport("Value", 21);

            var import = ImportDefinitionFactory.Create("Value", true);
            var importer = PartFactory.CreateImporter(import);

            engine.SatisfyImports(importer, true);

            Assert.AreEqual(21, importer.GetImport(import));

            exportProvider.ReplaceExportValue("Value", 42);

            Assert.AreEqual(42, importer.GetImport(import), "Value should change!");

            engine.UnregisterForRecomposition(importer);

            exportProvider.ReplaceExportValue("Value", 666);

            Assert.AreEqual(42, importer.GetImport(import), "Value should not change!");
        }
    }
}
