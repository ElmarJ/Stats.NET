// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Factories;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.UnitTesting;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.UnitTesting;

namespace System.ComponentModel.Composition
{
    [TestClass]
    public class CompositionContainerImportTests
    {
        // Exporting collectin values is not supported
        [TestMethod]
        public void ImportValues()
        {
            var container = ContainerFactory.Create();
            Importer importer = new Importer();
            Exporter exporter42 = new Exporter(42);

            CompositionBatch batch = new CompositionBatch();
            batch.AddPart(importer);
            batch.AddPart(exporter42);

            CompositionAssert.ThrowsError(ErrorId.CompositionEngine_PartCannotSetImport,
                                          ErrorId.ReflectionModel_ImportNotAssignableFromExport, RetryMode.DoNotRetry, () =>
            {
                container.Compose(batch);
            });            
        }

        [TestMethod]
        public void ImportSingle()
        {
            var container = ContainerFactory.Create();
            var importer = new Int32Importer();
            var exporter = new Int32Exporter(42);

            CompositionBatch batch = new CompositionBatch();
            batch.AddPart(importer);
            batch.AddPart(exporter);
            container.Compose(batch);

            Assert.AreEqual(42, importer.Value, "Expecting value to be imported");

        }

        [TestMethod]
        public void ImportSingleFromInternal()
        {
            var container = ContainerFactory.Create();
            var importer = new Int32Importer();
            var exporter = new Int32ExporterInternal(42);

            CompositionBatch batch = new CompositionBatch();
            batch.AddPart(importer);
            batch.AddPart(exporter);
            container.Compose(batch);

            Assert.AreEqual(42, importer.Value, "Expecting value to be imported");
        }

        [TestMethod]
        public void ImportSingleToInternal()
        {
            var container = ContainerFactory.Create();
            var importer = new Int32ImporterInternal();
            var exporter = new Int32Exporter(42);

            CompositionBatch batch = new CompositionBatch();
            batch.AddPart(importer);
            batch.AddPart(exporter);
            container.Compose(batch);

            Assert.AreEqual(42, importer.Value, "Expecting value to be imported");
        }

        [TestMethod]
        public void ImportSingleIntoCollection()
        {
            var container = ContainerFactory.Create();
            var importer = new Int32CollectionImporter();
            var exporter = new Int32Exporter(42);

            CompositionBatch batch = new CompositionBatch();
            batch.AddPart(importer);
            batch.AddPart(exporter);
            container.Compose(batch);

            EnumerableAssert.AreEqual(importer.Values, 42);
        }

        [TestMethod]
        public void ImportValuesNameless()
        {
            var container = ContainerFactory.Create();
            ImporterNameless importer;
            ExporterNameless exporter42 = new ExporterNameless(42);

            CompositionBatch batch = new CompositionBatch();
            batch.AddPart(importer = new ImporterNameless());
            batch.AddPart(exporter42);
            container.Compose(batch);

            Assert.AreEqual(42, importer.ValueReadWrite);
            Assert.AreEqual(42, importer.MetadataReadWrite.GetExportedObject());
        }

        [TestMethod]
        public void ImportValueExceptionMissing()
        {
            var container = ContainerFactory.Create();
            Importer importer;

            CompositionBatch batch = new CompositionBatch();
            batch.AddPart(importer = new Importer());

            CompositionAssert.ThrowsError(ErrorId.CompositionEngine_PartCannotSetImport,
                                          ErrorId.CompositionEngine_ImportCardinalityMismatch, 
                                          RetryMode.DoNotRetry, () =>
            {
                container.Compose(batch);
            });
        }

        [TestMethod]
        public void ImportValueExceptionMultiple()
        {
            var container = ContainerFactory.Create();
            Importer importer = new Importer();
            Exporter exporter42 = new Exporter(42);
            Exporter exporter6 = new Exporter(6);

            CompositionBatch batch = new CompositionBatch();
            batch.AddPart(importer);
            batch.AddPart(exporter42);
            batch.AddPart(exporter6);

            CompositionAssert.ThrowsErrors(ErrorId.CompositionEngine_PartCannotSetImport, ErrorId.CompositionEngine_PartCannotSetImport, RetryMode.DoNotRetry, () =>
            {
                container.Compose(batch);
            });
        }

        [TestMethod]
        public void ImportValueExceptionSetterException()
        {
            var container = ContainerFactory.Create();
            ImporterInvalidSetterException importer = new ImporterInvalidSetterException();
            Exporter exporter42 = new Exporter(42);

            CompositionBatch batch = new CompositionBatch();
            batch.AddPart(importer);
            batch.AddPart(exporter42);

            CompositionAssert.ThrowsError(ErrorId.CompositionEngine_PartCannotActivate,
                                          ErrorId.ReflectionModel_ImportThrewException, 
                                          RetryMode.DoNotRetry, () =>
            {
                container.Compose(batch);
            });
        }

        [TestMethod]
        public void ImportValueExceptionLazily()
        {
            var catalog = new AssemblyCatalog(typeof(ImportImporterInvalidSetterExceptionLazily).Assembly);
            var container = ContainerFactory.CreateWithAttributedCatalog(typeof(ImportImporterInvalidSetterExceptionLazily), typeof(ImporterInvalidSetterException));
            var invalidLazy = container.GetExportedObject<ImportImporterInvalidSetterExceptionLazily>();

            CompositionAssert.ThrowsError(ErrorId.CompositionEngine_PartCannotGetExportedObject,
                                          ErrorId.CompositionEngine_PartCannotActivate,
                                          ErrorId.ReflectionModel_ImportThrewException, RetryMode.DoNotRetry, () =>
            {
                invalidLazy.Value.GetExportedObject();
            });
        }

#if !SILVERLIGHT

        [TestMethod]
        public void ImportValueComComponent()
        {
            CTaskScheduler scheduler = new CTaskScheduler();

            try
            {
                var container = ContainerFactory.Create();
                var importer = new ImportComComponent();

                CompositionBatch batch = new CompositionBatch();
                batch.AddParts(importer);
                batch.AddExportedObject<ITaskScheduler>("TaskScheduler", (ITaskScheduler)scheduler);

                container.Compose(batch);

                Assert.AreEqual(scheduler, importer.TaskScheduler);
            }
            finally
            {
                Marshal.ReleaseComObject(scheduler);
            }
        }

        [TestMethod]
        public void DelayImportValueComComponent()
        {
            CTaskScheduler scheduler = new CTaskScheduler();

            try
            {
                var container = ContainerFactory.Create();
                var importer = new DelayImportComComponent();

                CompositionBatch batch = new CompositionBatch();
                batch.AddParts(importer);
                batch.AddExportedObject<ITaskScheduler>("TaskScheduler", (ITaskScheduler)scheduler);

                container.Compose(batch);

                Assert.AreEqual(scheduler, importer.TaskScheduler.GetExportedObject());
            }
            finally
            {
                Marshal.ReleaseComObject(scheduler);
            }
        }
#endif
       
        [TestMethod]
        [TestProperty("Type", "Integration")]
        public void OptionalImportsOfValueTypesAreBoundToDefaultWhenNotSatisfied()
        {
            var container = ContainerFactory.Create();
            var importer = new OptionalImport();

            CompositionBatch batch = new CompositionBatch();
            batch.AddPart(importer);
            container.Compose(batch);

            Assert.AreEqual(1, importer.ValueTypeSetCount);
            Assert.AreEqual(0, importer.ValueType);
        }

        [TestMethod]
        [TestProperty("Type", "Integration")]
        public void OptionalImportsOfNullableValueTypesAreBoundToDefaultWhenNotSatisfied()
        {
            var container = ContainerFactory.Create();
            var importer = new OptionalImport();

            CompositionBatch batch = new CompositionBatch();
            batch.AddPart(importer);
            container.Compose(batch);

            Assert.AreEqual(1, importer.NullableValueTypeSetCount);
            Assert.IsNull(importer.NullableValueType);
        }

        [TestMethod]
        [TestProperty("Type", "Integration")]
        public void OptionalImportsOfReferenceTypesAreBoundToDefaultWhenNotSatisfied()
        {
            var container = ContainerFactory.Create();
            var importer = new OptionalImport();

            CompositionBatch batch = new CompositionBatch();
            batch.AddPart(importer);
            container.Compose(batch);

            Assert.AreEqual(1, importer.ReferenceTypeSetCount);
            Assert.IsNull(importer.ReferenceType);
        }

        [TestMethod]
        [TestProperty("Type", "Integration")]
        public void OptionalImportsOfExportValueTypesAreBoundToDefaultWhenNotSatisfied()
        {
            var container = ContainerFactory.Create();
            var importer = new OptionalExport();

            CompositionBatch batch = new CompositionBatch();
            batch.AddPart(importer);
            container.Compose(batch);

            Assert.AreEqual(1, importer.ValueTypeSetCount);
            Assert.IsNull(importer.ValueType);
        }

        [TestMethod]
        [TestProperty("Type", "Integration")]
        public void OptionalImportsOfExportNullableValueTypesAreBoundToDefaultWhenNotSatisfied()
        {
            var container = ContainerFactory.Create();
            var importer = new OptionalExport();

            CompositionBatch batch = new CompositionBatch();
            batch.AddPart(importer);
            container.Compose(batch);

            Assert.AreEqual(1, importer.NullableValueTypeSetCount);
            Assert.IsNull(importer.NullableValueType);
        }

        [TestMethod]
        [TestProperty("Type", "Integration")]
        public void OptionalImportsOfExportReferenceTypesAreBoundToDefaultWhenNotSatisfied()
        {
            var container = ContainerFactory.Create();
            var importer = new OptionalExport();

            CompositionBatch batch = new CompositionBatch();
            batch.AddPart(importer);
            container.Compose(batch);

            Assert.AreEqual(1, importer.ReferenceTypeSetCount);
            Assert.IsNull(importer.ReferenceType);
        }

        [TestMethod]
        [TestProperty("Type", "Integration")]
        public void OptionalImportsOfValueTypesAreReboundToDefaultWhenExportIsRemoved()
        {
            var container = ContainerFactory.Create();
            var importer = new OptionalImport();

            CompositionBatch batch = new CompositionBatch();
            batch.AddPart(importer);
            var key = batch.AddExportedObject("ValueType", 10);

            container.Compose(batch);

            Assert.AreEqual(1, importer.ValueTypeSetCount);
            Assert.AreEqual(10, importer.ValueType);

            batch = new CompositionBatch();
            batch.RemovePart(key);
            container.Compose(batch);

            Assert.AreEqual(2, importer.ValueTypeSetCount);
            Assert.AreEqual(0, importer.ValueType);
        }

        [TestMethod]
        [TestProperty("Type", "Integration")]
        public void OptionalImportsOfNullableValueTypesAreReboundToDefaultWhenExportIsRemoved()
        {
            var container = ContainerFactory.Create();
            var importer = new OptionalImport();

            CompositionBatch batch = new CompositionBatch();
            batch.AddPart(importer);
            var key = batch.AddExportedObject<int?>("NullableValueType", 10);

            container.Compose(batch);
            Assert.AreEqual(1, importer.NullableValueTypeSetCount);
            Assert.AreEqual(10, importer.NullableValueType);

            batch = new CompositionBatch();
            batch.RemovePart(key);
            container.Compose(batch);

            Assert.AreEqual(2, importer.NullableValueTypeSetCount);
            Assert.IsNull(importer.NullableValueType);
        }

        [TestMethod]
        [TestProperty("Type", "Integration")]
        public void OptionalImportsOfReferenceTypesAreReboundToDefaultWhenExportIsRemoved()
        {
            var container = ContainerFactory.Create();
            var importer = new OptionalImport();

            CompositionBatch batch = new CompositionBatch();
            batch.AddPart(importer);
            var key = batch.AddExportedObject("ReferenceType", "Bar");

            container.Compose(batch);
            Assert.AreEqual(1, importer.ReferenceTypeSetCount);
            Assert.AreEqual("Bar", importer.ReferenceType);

            batch = new CompositionBatch();
            batch.RemovePart(key);
            container.Compose(batch);

            Assert.AreEqual(2, importer.ReferenceTypeSetCount);
            Assert.IsNull(importer.ReferenceType);
        }

        [TestMethod]
        [TestProperty("Type", "Integration")]
        public void OptionalImportsOfExportValueTypesAreReboundToDefaultWhenExportIsRemoved()
        {
            var container = ContainerFactory.Create();
            var importer = new OptionalExport();

            CompositionBatch batch = new CompositionBatch();
            batch.AddPart(importer);
            var key = batch.AddExportedObject("ValueType", 10);

            container.Compose(batch);

            Assert.AreEqual(1, importer.ValueTypeSetCount);
            Assert.AreEqual(10, importer.ValueType.GetExportedObject());

            batch = new CompositionBatch();
            batch.RemovePart(key);
            container.Compose(batch);

            Assert.AreEqual(2, importer.ValueTypeSetCount);
            Assert.IsNull(importer.ValueType);
        }

        [TestMethod]
        [TestProperty("Type", "Integration")]
        public void OptionalImportsOfExportNullableValueTypesAreReboundToDefaultWhenExportIsRemoved()
        {
            var container = ContainerFactory.Create();
            var importer = new OptionalExport();

            CompositionBatch batch = new CompositionBatch();
            batch.AddPart(importer);
            var key = batch.AddExportedObject<int?>("NullableValueType", 10);

            container.Compose(batch);
            Assert.AreEqual(1, importer.NullableValueTypeSetCount);
            Assert.AreEqual(10, importer.NullableValueType.GetExportedObject());

            batch = new CompositionBatch();
            batch.RemovePart(key);
            container.Compose(batch);

            Assert.AreEqual(2, importer.NullableValueTypeSetCount);
            Assert.IsNull(importer.NullableValueType);
        }

        [TestMethod]
        [TestProperty("Type", "Integration")]
        public void OptionalImportsOfExportReferenceTypesAreReboundToDefaultWhenExportIsRemoved()
        {
            var container = ContainerFactory.Create();
            var importer = new OptionalExport();

            CompositionBatch batch = new CompositionBatch();
            batch.AddPart(importer);
            var key = batch.AddExportedObject("ReferenceType", "Bar");

            container.Compose(batch);
            Assert.AreEqual(1, importer.ReferenceTypeSetCount);
            Assert.AreEqual("Bar", importer.ReferenceType.GetExportedObject());

            batch = new CompositionBatch();
            batch.RemovePart(key);
            container.Compose(batch);

            Assert.AreEqual(2, importer.ReferenceTypeSetCount);
            Assert.IsNull(importer.ReferenceType);
        }

        public class OptionalImport
        {
            public int ValueTypeSetCount;
            public int NullableValueTypeSetCount;
            public int ReferenceTypeSetCount;

            private int _valueType;
            private int? _nullableValueType;
            private string _referenceType;

            [Import("ValueType", AllowDefault = true, AllowRecomposition = true)]
            public int ValueType
            {
                get { return _valueType; }
                set
                {
                    ValueTypeSetCount++;
                    _valueType = value;
                }
            }

            [Import("NullableValueType", AllowDefault = true, AllowRecomposition = true)]
            public int? NullableValueType
            {
                get { return _nullableValueType; }
                set
                {
                    NullableValueTypeSetCount++;
                    _nullableValueType = value;
                }
            }

            [Import("ReferenceType", AllowDefault = true, AllowRecomposition = true)]
            public string ReferenceType
            {
                get { return _referenceType; }
                set 
                {
                    ReferenceTypeSetCount++;
                    _referenceType = value; 
                }
            }
        }

        public class OptionalExport
        {
            public int ValueTypeSetCount;
            public int NullableValueTypeSetCount;
            public int ReferenceTypeSetCount;

            private Export<int> _valueType;
            private Export<int?> _nullableValueType;
            private Export<string> _referenceType;

            [Import("ValueType", AllowDefault = true, AllowRecomposition = true)]
            public Export<int> ValueType
            {
                get { return _valueType; }
                set
                {
                    ValueTypeSetCount++;
                    _valueType = value;
                }
            }

            [Import("NullableValueType", AllowDefault = true, AllowRecomposition = true)]
            public Export<int?> NullableValueType
            {
                get { return _nullableValueType; }
                set
                {
                    NullableValueTypeSetCount++;
                    _nullableValueType = value;
                }
            }

            [Import("ReferenceType", AllowDefault = true, AllowRecomposition = true)]
            public Export<string> ReferenceType
            {
                get { return _referenceType; }
                set
                {
                    ReferenceTypeSetCount++;
                    _referenceType = value;
                }
            }
        }

        private class DelayDuckImporter
        {
            [Import("Duck")]
            public Export<IDuck> Duck
            {
                get;
                set;
            }
        }

        private class DuckImporter
        {
            [Import("Duck")]
            public IDuck Duck
            {
                get;
                set;
            }
        }

        public class QuackLikeADuck
        {
            public virtual string Quack()
            {
                return "Quack";
            }
        }

        public interface IDuck
        {
            string Quack();            
        }

        #if !SILVERLIGHT

        [ComImport]
        [Guid("148BD52A-A2AB-11CE-B11F-00AA00530503")]
        private class CTaskScheduler
        {   // This interface doesn't implement 
            // ITaskScheduler deliberately
        }

        [Guid("148BD527-A2AB-11CE-B11F-00AA00530503")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface ITaskScheduler
        {
            void FakeMethod();
        }

        private class ImportComComponent
        {
            [Import("TaskScheduler")]
            public ITaskScheduler TaskScheduler
            {
                get;
                set;
            }
        }

        private class DelayImportComComponent
        {
            [Import("TaskScheduler")]
            public Export<ITaskScheduler> TaskScheduler
            {
                get;
                set;
            }
        }

#endif

        public class Importer
        {
            public Importer()
            {
            }

            [Import("Value")]
            public int ValueReadWrite { get; set; }

            [ImportMany("Value")]
            public IList<int> SingleValueCollectionReadWrite { get; set; }

            [Import("EmptyValue", AllowDefault = true)]
            public int ValueEmptyOptional { get; set; }

            [ImportMany("CollectionValue", typeof(IList<int>))]
            public IList<int> ValueCollection { get; set; }

        }

        public class ImporterNameless
        {

            public ImporterNameless()
            {
            }

            [Import]
            public int ValueReadWrite { get; set; }

            [Import]
            public Export<int> MetadataReadWrite { get; set; }

        }

        public class ImporterInvalidWrongType
        {
            [Import("Value")]
            public DateTime ValueReadWrite { get; set; }
        }

        [Export]
        public class ImporterInvalidSetterException
        {
            [ImportMany("Value")]
            public IEnumerable<int> ValueReadWrite { get { return null; } set { throw new InvalidOperationException(); } }
        }

        [Export]
        public class ImportImporterInvalidSetterExceptionLazily
        {
            [Import]
            public Export<ImporterInvalidSetterException> Value { get; set; }
        }


        [PartNotDiscoverable]
        public class Exporter
        {
            List<int> collectionValue = new List<int>();

            public Exporter(int value)
            {
                Value = value;
            }

            [Export("Value")]
            public int Value { get; set; }


            [Export("CollectionValue")]
            public IList<int> CollectionValue { get { return collectionValue; } }

        }


        public class ExporterNameless
        {

            public ExporterNameless(int value)
            {
                Value = value;
            }

            [Export]
            public int Value { get; set; }

        }


    }
}
