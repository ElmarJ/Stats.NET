// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Integration
{
    [TestClass]
    public class RecompositionTests
    {
        public class Class_OptIn_AllowRecompositionImports
        {
            [Import("Value", AllowRecomposition = true)]
            public int Value { get; set; }
        }

        [TestMethod]
        public void Import_OptIn_AllowRecomposition()
        {
            var container = new CompositionContainer();
            var importer = new Class_OptIn_AllowRecompositionImports();

            CompositionBatch batch = new CompositionBatch();
            batch.AddPart(importer);
            var valueKey = batch.AddExportedObject("Value", 21);
            container.Compose(batch);

            // Initial compose Value should be 21
            Assert.AreEqual(21, importer.Value);

            // Recompose Value to be 42
            batch = new CompositionBatch();
            batch.RemovePart(valueKey);
            batch.AddExportedObject("Value", 42);
            container.Compose(batch);

            Assert.AreEqual(42, importer.Value, "Value should have changed!");
        }

        public class Class_OptOut_AllowRecompositionImports
        {
            [Import("Value", AllowRecomposition = false)]
            public int Value { get; set; }
        }

        [TestMethod]
        public void Import_OptOut_AllowRecomposition()
        {
            var container = new CompositionContainer();
            var importer = new Class_OptOut_AllowRecompositionImports();

            CompositionBatch batch = new CompositionBatch();
            batch.AddPart(importer);
            var valueKey = batch.AddExportedObject("Value", 21);
            container.Compose(batch);

            // Initial compose Value should be 21
            Assert.AreEqual(21, importer.Value);

            // Reset value to ensure it doesn't get set to same value again
            importer.Value = -21; 

            // Recompose Value to be 42
            batch = new CompositionBatch();
            batch.RemovePart(valueKey);
            batch.AddExportedObject("Value", 42);
            container.Compose(batch);

            Assert.AreEqual(-21, importer.Value, "Value should NOT have changed!");
        }

        public class Class_Default_AllowRecompositionImports
        {
            [Import("Value")]
            public int Value { get; set; }
        }

        [TestMethod]
        public void Import_Default_AllowRecomposition()
        {
            var container = new CompositionContainer();
            var importer = new Class_Default_AllowRecompositionImports();

            CompositionBatch batch = new CompositionBatch();
            batch.AddPart(importer);
            var valueKey = batch.AddExportedObject("Value", 21);
            container.Compose(batch);

            // Initial compose Value should be 21
            Assert.AreEqual(21, importer.Value);

            // Reset value to ensure it doesn't get set to same value again
            importer.Value = -21; 

            // Recompose Value to be 42
            batch = new CompositionBatch();
            batch.RemovePart(valueKey);
            batch.AddExportedObject("Value", 42);
            container.Compose(batch);

            Assert.AreEqual(-21, importer.Value, "Value should NOT have changed!");
        }

        public class Class_BothOptInAndOptOutRecompositionImports
        {
            [Import("Value", AllowRecomposition = true)]
            public int RecomposableValue { get; set; }

            [Import("Value", AllowRecomposition = false)]
            public int NonRecomposableValue { get; set; }
        }

        [TestMethod]
        public void Import_BothOptInAndOptOutRecomposition()
        {
            var container = new CompositionContainer();
            var importer = new Class_BothOptInAndOptOutRecompositionImports();

            CompositionBatch batch = new CompositionBatch();
            batch.AddPart(importer);
            var valueKey = batch.AddExportedObject("Value", 21);
            container.Compose(batch);

            // Initial compose values should be 21
            Assert.AreEqual(21, importer.RecomposableValue);
            Assert.AreEqual(21, importer.NonRecomposableValue);

            // Reset value to ensure it doesn't get set to same value again
            importer.NonRecomposableValue = -21;

            // Recompose Value to be 42
            batch = new CompositionBatch();
            batch.RemovePart(valueKey);
            batch.AddExportedObject("Value", 42);
            container.Compose(batch);

            Assert.AreEqual(-21, importer.NonRecomposableValue, "Value should NOT have changed!");
            Assert.AreEqual(42, importer.RecomposableValue, "Value should have changed!");
        }

        public class Class_MultipleOptInRecompositionImportsWithDifferentContracts
        {
            [Import("Value1", AllowRecomposition = true)]
            public int Value1 { get; set; }

            [Import("Value2", AllowRecomposition = true)]
            public int Value2 { get; set; }
        }

        [TestMethod]
        public void Import_OptInRecomposition_Multlple()
        {
            var container = new CompositionContainer();
            var importer = new Class_MultipleOptInRecompositionImportsWithDifferentContracts();

            CompositionBatch batch = new CompositionBatch();
            batch.AddPart(importer);
            var value1Key = batch.AddExportedObject("Value1", 21);
            var value2Key = batch.AddExportedObject("Value2", 23);
            container.Compose(batch);

            Assert.AreEqual(21, importer.Value1);
            Assert.AreEqual(23, importer.Value2);

            // Reset value to ensure it doesn't get set to same value again
            importer.Value1 = -21;
            importer.Value2 = -23;

            // Recompose Value to be 42
            batch = new CompositionBatch();
            batch.RemovePart(value1Key);
            batch.AddExportedObject("Value1", 42);
            container.Compose(batch);

            Assert.AreEqual(42, importer.Value1, "Value should have changed!");
            
            // Currently we recompose all recomposable imports on a part when any of the
            // imports on that part get recomposed. In the future we may consider actually only
            // composing changed imports but the current composition engine doesn't support it.
            Assert.AreEqual(23, importer.Value2, "Value should have changed to the value in the container.");
        }
    }
}
