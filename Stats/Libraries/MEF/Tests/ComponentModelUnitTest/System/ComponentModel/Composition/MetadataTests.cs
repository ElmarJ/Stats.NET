// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Factories;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.UnitTesting;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.UnitTesting;
using System.Reflection;

namespace System.ComponentModel.Composition
{
    [TestClass]
    public class MetadataTests
    {
        #region Tests for metadata on exports

        [TestMethod]
        public void ValidMetadataTest()
        {
            var container = ContainerFactory.Create();
            CompositionBatch batch = new CompositionBatch();
            batch.AddPart(new MyExporterWithValidMetadata());
            container.Compose(batch);
            
            var typeVi = container.GetExport<MyExporterWithValidMetadata>();
            var metadataFoo = typeVi.Metadata["foo"] as IList<string>;
            Assert.AreEqual(2, metadataFoo.Count(), "There are should be two items in the metadata foo's collection");
            Assert.IsTrue(metadataFoo.Contains("bar1"), "The metadata collection should include value 'bar1'");
            Assert.IsTrue(metadataFoo.Contains("bar2"), "The metadata collection should include value 'bar2'");
            Assert.AreEqual("world", typeVi.Metadata["hello"], "The single item metadata should be present");
            Assert.AreEqual("GoodOneValue2", typeVi.Metadata["GoodOne2"], "The metadata supplied by strong attribute should also be present");

            var metadataAcme = typeVi.Metadata["acme"] as IList<object>;
            Assert.AreEqual(2, metadataAcme.Count(), "There are should be two items in the metadata acme's collection");
            Assert.IsTrue(metadataAcme.Contains("acmebar"), "The metadata collection should include value 'bar'");
            Assert.IsTrue(metadataAcme.Contains(2.0), "The metadata collection should include value 2");
           
            var memberVi = container.GetExport<Func<double>>("ContractForValidMetadata");
            var metadataBar = memberVi.Metadata["bar"] as IList<string>;
            Assert.AreEqual(2, metadataBar.Count(), "There are should be two items in the metadata bar's collection");
            Assert.IsTrue(metadataBar.Contains("foo1"), "The metadata collection should include value 'foo1'");
            Assert.IsTrue(metadataBar.Contains("foo2"), "The metadata collection should include value 'foo2'");
            Assert.AreEqual("hello", memberVi.Metadata["world"], "The single item metadata should be present");
            Assert.AreEqual("GoodOneValue2", memberVi.Metadata["GoodOne2"], "The metadata supplied by strong attribute should also be present");

            var metadataStuff = memberVi.Metadata["stuff"] as IList<object>;
            Assert.AreEqual(2, metadataAcme.Count(), "There are should be two items in the metadata acme's collection");
            Assert.IsTrue(metadataStuff.Contains("acmebar"), "The metadata collection should include value 'acmebar'");
            Assert.IsTrue(metadataStuff.Contains(2.0), "The metadata collection should include value 2");

        }

        [TestMethod]
        public void ValidMetadataDiscoveredByComponentCatalogTest()
        {
            var container = ContainerFactory.CreateWithDefaultAttributedCatalog();
            ValidMetadataDiscoveredByCatalog(container);
        }

        private void ValidMetadataDiscoveredByCatalog(CompositionContainer container)
        {
            var export1 = container.GetExport<MyExporterWithValidMetadata>();
            
            var metadataFoo = export1.Metadata["foo"] as IList<string>;
            Assert.AreEqual(2, metadataFoo.Count(), "There are should be two items in the metadata foo's collection");
            Assert.IsTrue(metadataFoo.Contains("bar1"), "The metadata collection should include value 'bar1'");
            Assert.IsTrue(metadataFoo.Contains("bar2"), "The metadata collection should include value 'bar2'");
            Assert.AreEqual("world", export1.Metadata["hello"], "The single item metadata should also be present");
            Assert.AreEqual("GoodOneValue2", export1.Metadata["GoodOne2"], "The metadata supplied by strong attribute should also be present");

            var metadataAcme = export1.Metadata["acme"] as IList<object>;
            Assert.AreEqual(2, metadataAcme.Count(), "There are should be two items in the metadata acme's collection");
            Assert.IsTrue(metadataAcme.Contains("acmebar"), "The metadata collection should include value 'bar'");
            Assert.IsTrue(metadataAcme.Contains(2.0), "The metadata collection should include value 2");

            var export2 = container.GetExport<Func<double>>("ContractForValidMetadata");
            var metadataBar = export2.Metadata["bar"] as IList<string>;
            Assert.AreEqual(2, metadataBar.Count(), "There are should be two items in the metadata foo's collection");
            Assert.IsTrue(metadataBar.Contains("foo1"), "The metadata collection should include value 'foo1'");
            Assert.IsTrue(metadataBar.Contains("foo2"), "The metadata collection should include value 'foo2'");
            Assert.AreEqual("hello", export2.Metadata["world"], "The single item metadata should also be present");
            Assert.AreEqual("GoodOneValue2", export2.Metadata["GoodOne2"], "The metadata supplied by strong attribute should also be present");

            var metadataStuff = export2.Metadata["stuff"] as IList<object>;
            Assert.AreEqual(2, metadataAcme.Count(), "There are should be two items in the metadata acme's collection");
            Assert.IsTrue(metadataStuff.Contains("acmebar"), "The metadata collection should include value 'acmebar'");
            Assert.IsTrue(metadataStuff.Contains(2.0), "The metadata collection should include value 2");
        }


        [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
        [MetadataAttribute]
        public class BadStrongMetadata : Attribute
        {
            public string SelfConflicted { get { return "SelfConflictedValue"; } }
        }

        [Export]
        [BadStrongMetadata]
        [ExportMetadata("InvalidCollection", "InvalidCollectionValue1")]
        [ExportMetadata("InvalidCollection", "InvalidCollectionValue2", IsMultiple = true)]
        [BadStrongMetadata]
        [ExportMetadata("RepeatedMetadata", "RepeatedMetadataValue1")]
        [ExportMetadata("RepeatedMetadata", "RepeatedMetadataValue2")]
        [ExportMetadata("GoodOne1", "GoodOneValue1")]
        [ExportMetadata("ConflictedOne1", "ConfilictedOneValue1")]
        [GoodStrongMetadata]
        [ExportMetadata("ConflictedOne2", "ConflictedOne2Value2")]
        [PartNotDiscoverable]
        public class MyExporterWithInvalidMetadata
        {
            [Export("ContractForInvalidMetadata")]
            [ExportMetadata("ConflictedOne1", "ConfilictedOneValue1")]
            [GoodStrongMetadata]
            [ExportMetadata("ConflictedOne2", "ConflictedOne2Value2")]
            [ExportMetadata("RepeatedMetadata", "RepeatedMetadataValue1")]
            [ExportMetadata("RepeatedMetadata", "RepeatedMetadataValue2")]
            [BadStrongMetadata]
            [ExportMetadata("InvalidCollection", "InvalidCollectionValue1")]
            [ExportMetadata("InvalidCollection", "InvalidCollectionValue2", IsMultiple = true)]
            [BadStrongMetadata]
            [ExportMetadata("GoodOne1", "GoodOneValue1")]
            public double DoSomething() { return 0.618; }
        }

        [Export]
        [ExportMetadata("DuplicateMetadataName", "My Name")]
        [ExportMetadata("DuplicateMetadataName", "Your Name")]
        [PartNotDiscoverable]
        public class ClassWithInvalidDuplicateMetadataOnType
        {

        }

        [TestMethod]
        public void InvalidDuplicateMetadataOnType_ShouldThrow()
        {
            var part = AttributedModelServices.CreatePart(new ClassWithInvalidDuplicateMetadataOnType());
            var export = part.ExportDefinitions.First();
            var ex = ExceptionAssert.Throws<InvalidOperationException>(RetryMode.DoNotRetry, () =>
            {
                var metadata = export.Metadata;
            });

            Assert.IsTrue(ex.Message.Contains("DuplicateMetadataName"));
        }

        [PartNotDiscoverable]
        public class ClassWithInvalidDuplicateMetadataOnMember
        {
            [Export]
            [ExportMetadata("DuplicateMetadataName", "My Name")]
            [ExportMetadata("DuplicateMetadataName", "Your Name")]
            public ClassWithDuplicateMetadataOnMember Member { get; set; }
        }

        [TestMethod]
        public void InvalidDuplicateMetadataOnMember_ShouldThrow()
        {
            var part = AttributedModelServices.CreatePart(new ClassWithInvalidDuplicateMetadataOnMember());
            var export = part.ExportDefinitions.First();

            var ex = ExceptionAssert.Throws<InvalidOperationException>(RetryMode.DoNotRetry, () =>
            {
                var metadata = export.Metadata;
            });

            Assert.IsTrue(ex.Message.Contains("DuplicateMetadataName"));
        }

        [Export]
        [ExportMetadata("DuplicateMetadataName", "My Name", IsMultiple=true)]
        [ExportMetadata("DuplicateMetadataName", "Your Name", IsMultiple=true)]
        public class ClassWithValidDuplicateMetadataOnType
        {

        }

        [TestMethod]
        public void ValidDuplicateMetadataOnType_ShouldDiscoverAllMetadata()
        {
            var container = ContainerFactory.Create();
            CompositionBatch batch = new CompositionBatch();
            batch.AddPart(new ClassWithValidDuplicateMetadataOnType());

            container.Compose(batch);

            var export = container.GetExport<ClassWithValidDuplicateMetadataOnType>();

            var names = export.Metadata["DuplicateMetadataName"] as string[];

            Assert.AreEqual(2, names.Length);
        }

        public class ClassWithDuplicateMetadataOnMember
        {
            [Export]
            [ExportMetadata("DuplicateMetadataName", "My Name", IsMultiple=true)]
            [ExportMetadata("DuplicateMetadataName", "Your Name", IsMultiple=true)]
            public ClassWithDuplicateMetadataOnMember Member { get; set; }
        }

        [TestMethod]
        public void ValidDuplicateMetadataOnMember_ShouldDiscoverAllMetadata()
        {
            var container = ContainerFactory.Create();
            CompositionBatch batch = new CompositionBatch();
            batch.AddPart(new ClassWithDuplicateMetadataOnMember());

            container.Compose(batch);

            var export = container.GetExport<ClassWithDuplicateMetadataOnMember>();

            var names = export.Metadata["DuplicateMetadataName"] as string[];

            Assert.AreEqual(2, names.Length);
        }

        [Export]
        [ExportMetadata(CompositionConstants.PartCreationPolicyMetadataName, "My Policy")]
        [PartNotDiscoverable]
        public class ClassWithReservedMetadataValue
        {

        }

        [TestMethod]
        public void InvalidMetadata_UseOfReservedName_ShouldThrow()
        {
            var part = AttributedModelServices.CreatePart(new ClassWithReservedMetadataValue());
            var export = part.ExportDefinitions.First();

            var ex = ExceptionAssert.Throws<InvalidOperationException>(RetryMode.DoNotRetry, () =>
            {
                var metadata = export.Metadata;
            });

            Assert.IsTrue(ex.Message.Contains(CompositionConstants.PartCreationPolicyMetadataName));
        }

        #endregion

        #region Tests for weakly supported metadata as part of contract

        [TestMethod]
        [WorkItem(468388)]
        [Ignore]
        public void FailureImportForNoRequiredMetadatForExportCollection()
        {
            CompositionContainer container = ContainerFactory.Create();

            MyImporterWithExportCollection importer;
            CompositionBatch batch = new CompositionBatch();
            batch.AddPart(new MyExporterWithNoMetadata());
            batch.AddPart(importer = new MyImporterWithExportCollection());

            Assert.Fail();

            //var result = container.TryCompose();

            //Assert.IsTrue(result.Succeeded, "Composition should be successful because collection import is not required");
            //Assert.AreEqual(1, result.Issues.Count, "There should be one issue reported");
            //Assert.IsTrue(result.Issues[0].Description.Contains("Foo"), "The missing required metadata is 'Foo'");
        }

        [TestMethod]
        [WorkItem(472538)]
        [Ignore]
        public void FailureImportForNoRequiredMetadataThroughComponentCatalogTest()
        {
            var container = ContainerFactory.CreateWithDefaultAttributedCatalog();
            FailureImportForNoRequiredMetadataThroughCatalog(container);
        }

        private void FailureImportForNoRequiredMetadataThroughCatalog(CompositionContainer container)
        {
            Assert.Fail("This needs to be fixed, see: 472538");

            //var export1 = container.GetExport<MyImporterWithExport>();

            //export1.TryGetExportedObject().VerifyFailure(CompositionIssueId.RequiredMetadataNotFound, CompositionIssueId.CardinalityMismatch);

            //var export2 = container.GetExport<MyImporterWithExportCollection>();
            //export2.TryGetExportedObject().VerifySuccess(CompositionIssueId.RequiredMetadataNotFound);

            //container.TryGetExportedObject<MyImporterWithValue>().VerifyFailure(CompositionIssueId.RequiredMetadataNotFound, CompositionIssueId.CardinalityMismatch);
        }

        [TestMethod]
        [WorkItem(468388)]
        [Ignore]
        public void SelectiveImportBasedOnMetadataForExport()
        {
            CompositionContainer container = ContainerFactory.Create();

            MyImporterWithExportForSelectiveImport importer;
            CompositionBatch batch = new CompositionBatch();
            batch.AddPart(new MyExporterWithNoMetadata());
            batch.AddPart(new MyExporterWithMetadata());
            batch.AddPart(importer = new MyImporterWithExportForSelectiveImport());

            Assert.Fail();
            //var result = container.TryCompose();

            //Assert.IsTrue(result.Succeeded, "Composition should be successfull because one of two exports meets both the contract name and metadata requirement");
            //Assert.AreEqual(1, result.Issues.Count, "There should be one issue reported about the export who has no required metadata");
            //Assert.IsTrue(result.Issues[0].Description.Contains("Foo"), "The missing required metadata is 'Foo'");
            //Assert.IsNotNull(importer.ValueInfo, "The import should really get bound");
        }

        [TestMethod]
        [WorkItem(468388)]
        [Ignore]
        public void SelectiveImportBasedOnMetadataForExportCollection()
        {
            CompositionContainer container = ContainerFactory.Create();

            MyImporterWithExportCollectionForSelectiveImport importer;
            CompositionBatch batch = new CompositionBatch();
            batch.AddPart(new MyExporterWithNoMetadata());
            batch.AddPart(new MyExporterWithMetadata());
            batch.AddPart(importer = new MyImporterWithExportCollectionForSelectiveImport());

            Assert.Fail();

            //var result = container.TryCompose();

            //Assert.IsTrue(result.Succeeded, "Composition should be successfull in anyway for collection import");
            //Assert.AreEqual(1, result.Issues.Count, "There should be one issue reported however, about the export who has no required metadata");
            //Assert.IsTrue(result.Issues[0].Description.Contains("Foo"), "The missing required metadata is 'Foo'");
            //Assert.AreEqual(1, importer.ValueInfoCol.Count, "The export with required metadata should get bound");
            //Assert.IsNotNull(importer.ValueInfoCol[0], "The import should really get bound");
        }

        
        [TestMethod]
        [WorkItem(472538)]
        [Ignore]
        public void SelectiveImportBasedOnMetadataThruoughComponentCatalogTest()
        {
            var container = ContainerFactory.CreateWithDefaultAttributedCatalog();
            SelectiveImportBasedOnMetadataThruoughCatalog(container);
        }

        private void SelectiveImportBasedOnMetadataThruoughCatalog(CompositionContainer container)
        {
            Assert.Fail("This needs to be fixed, see: 472538");

            //var export1 = container.GetExport<MyImporterWithExportForSelectiveImport>();
            //export1.TryGetExportedObject().VerifySuccess(CompositionIssueId.RequiredMetadataNotFound);

            //var export2 = container.GetExport<MyImporterWithExportCollectionForSelectiveImport>();
            //export2.TryGetExportedObject().VerifySuccess(CompositionIssueId.RequiredMetadataNotFound);
        }

        [TestMethod]
        public void ChildParentContainerTest1()
        {
            CompositionContainer parent = ContainerFactory.Create();
            CompositionContainer child = new CompositionContainer(parent);

            CompositionBatch childBatch = new CompositionBatch();
            CompositionBatch parentBatch = new CompositionBatch();
            parentBatch.AddPart(new MyExporterWithNoMetadata());
            childBatch.AddPart(new MyExporterWithMetadata());
            parent.Compose(parentBatch);
            child.Compose(childBatch);

            var exports = child.GetExports(CreateImportDefinition(typeof(IMyExporter), "Foo"));

            Assert.AreEqual(1, exports.Count());
        }

        [TestMethod]
        public void ChildParentContainerTest2()
        {
            CompositionContainer parent = ContainerFactory.Create();
            CompositionContainer child = new CompositionContainer(parent);

            CompositionBatch childBatch = new CompositionBatch();
            CompositionBatch parentBatch = new CompositionBatch();
            parentBatch.AddPart(new MyExporterWithMetadata());
            childBatch.AddPart(new MyExporterWithNoMetadata());
            parent.Compose(parentBatch);

            var exports = child.GetExports(CreateImportDefinition(typeof(IMyExporter), "Foo"));

            Assert.AreEqual(1, exports.Count());
        }

        [TestMethod]
        public void ChildParentContainerTest3()
        {
            CompositionContainer parent = ContainerFactory.Create();
            CompositionContainer child = new CompositionContainer(parent);

            CompositionBatch childBatch = new CompositionBatch();
            CompositionBatch parentBatch = new CompositionBatch();

            parentBatch.AddPart(new MyExporterWithMetadata());
            childBatch.AddPart(new MyExporterWithMetadata());
            parent.Compose(parentBatch);
            child.Compose(childBatch);

            var exports = child.GetExports(CreateImportDefinition(typeof(IMyExporter), "Foo"));

            Assert.AreEqual(2, exports.Count(), "There should be two from child and parent container each");
        }

        private static ImportDefinition CreateImportDefinition(Type type, string metadataKey)
        {
            return new ContractBasedImportDefinition(AttributedModelServices.GetContractName(typeof(IMyExporter)), null, new string[] { metadataKey }, ImportCardinality.ZeroOrMore, true, true, CreationPolicy.Any);
        }

        #endregion

        #region Tests for strongly typed metadata as part of contract

#if !SILVERLIGHT
        // Silverlight does not support strongly typed metadata

        [TestMethod]
        [WorkItem(468388)]
        [Ignore]
        public void SelectiveImportBySTM_Export()
        {
            CompositionContainer container = ContainerFactory.Create();
            CompositionBatch batch = new CompositionBatch();

            MyImporterWithExportStronglyTypedMetadata importer;
            batch.AddPart(new MyExporterWithNoMetadata());
            batch.AddPart(new MyExporterWithMetadata());
            batch.AddPart(importer = new MyImporterWithExportStronglyTypedMetadata());

            Assert.Fail();

            //var result = container.TryCompose();

            //Assert.IsTrue(result.Succeeded, "Composition should be successful becasue one of two exports does not have required metadata");
            //Assert.AreEqual(1, result.Issues.Count, "There should be an issue reported about the export who has no required metadata");
            //Assert.IsNotNull(importer.ValueInfo, "The valid export should really get bound");
            //Assert.AreEqual("Bar", importer.ValueInfo.MetadataView.Foo, "The value of metadata 'Foo' should be 'Bar'");
        }

        [TestMethod]
        [WorkItem(468388)]
        [Ignore]
        public void SelectiveImportBySTM_ExportCollection()
        {
            CompositionContainer container = ContainerFactory.Create();

            MyImporterWithExportCollectionStronglyTypedMetadata importer;
            CompositionBatch batch = new CompositionBatch();

            batch.AddPart(new MyExporterWithNoMetadata());
            batch.AddPart(new MyExporterWithMetadata());
            batch.AddPart(importer = new MyImporterWithExportCollectionStronglyTypedMetadata());

            Assert.Fail();

            //var result = container.TryCompose();

            //Assert.IsTrue(result.Succeeded, "Collection import should be successful in anyway");
            //Assert.AreEqual(1, result.Issues.Count, "There should be an issue reported about the export with no required metadata");
            //Assert.AreEqual(1, importer.ValueInfoCol.Count, "There should be only one export got bound");
            //Assert.AreEqual("Bar", importer.ValueInfoCol.First().MetadataView.Foo, "The value of metadata 'Foo' should be 'Bar'");
        }

        [TestMethod]
        public void SelectiveImportBySTMThroughComponentCatalog1()
        {
            var container = ContainerFactory.CreateWithDefaultAttributedCatalog();
            SelectiveImportBySTMThroughCatalog1(container);
        }

        public void SelectiveImportBySTMThroughCatalog1(CompositionContainer container)
        {
            Assert.IsNotNull(container.GetExport<IMyExporter, IMetadataView>());

            var result2 = container.GetExports<IMyExporter, IMetadataView>();
        }

        [TestMethod]
        [WorkItem(468388)]
        [Ignore]
        public void SelectiveImportBySTMThroughComponentCatalog2()
        {
            var container = ContainerFactory.CreateWithDefaultAttributedCatalog();
            SelectiveImportBySTMThroughCatalog2(container);
        }

        public void SelectiveImportBySTMThroughCatalog2(CompositionContainer container)
        {
            Assert.Fail("This needs to be fixed, see: 472538");

            //var export1 = container.GetExport<MyImporterWithExportStronglyTypedMetadata>();
            //var result1 = export1.TryGetExportedObject().VerifySuccess(CompositionIssueId.RequiredMetadataNotFound);
            //Assert.IsNotNull(result1.Value.ValueInfo, "The valid export should really get bound");
            //Assert.AreEqual("Bar", result1.Value.ValueInfo.MetadataView.Foo, "The value of metadata 'Foo' should be 'Bar'");

            //var export2 = container.GetExport<MyImporterWithExportCollectionStronglyTypedMetadata>();
            //var result2 = export2.TryGetExportedObject().VerifySuccess(CompositionIssueId.RequiredMetadataNotFound);
            //Assert.AreEqual(1, result2.Value.ValueInfoCol.Count, "There should be only one export got bound");
            //Assert.AreEqual("Bar", result2.Value.ValueInfoCol.First().MetadataView.Foo, "The value of metadata 'Foo' should be 'Bar'");
        }

        [TestMethod]
        public void TestMultipleStronglyTypedAttributes()
        {
            var container = ContainerFactory.CreateWithDefaultAttributedCatalog();

            var export = container.GetExport<ExportMultiple, IMyOptions>();
            EnumerableAssert.AreEqual(export.MetadataView.OptionNames.OrderBy(s => s), "name1", "name2", "name3");
            EnumerableAssert.AreEqual(export.MetadataView.OptionValues.OrderBy(o => o.ToString()), "value1", "value2", "value3");
        }

        [TestMethod]
        public void TestMultipleStronglyTypedAttributesAsIEnumerable()
        {
            var container = ContainerFactory.CreateWithDefaultAttributedCatalog();

            var export = container.GetExport<ExportMultiple, IMyOptionsAsIEnumerable>();
            EnumerableAssert.AreEqual(export.MetadataView.OptionNames.OrderBy(s => s), "name1", "name2", "name3");
            EnumerableAssert.AreEqual(export.MetadataView.OptionValues.OrderBy(o => o.ToString()), "value1", "value2", "value3");
        }

        [TestMethod]
        public void TestMultipleStronglyTypedAttributesAsArray()
        {
            var container = ContainerFactory.CreateWithDefaultAttributedCatalog();

            var export = container.GetExport<ExportMultiple, IMyOptionsAsArray>();
            EnumerableAssert.AreEqual(export.MetadataView.OptionNames.OrderBy(s => s), "name1", "name2", "name3");
            EnumerableAssert.AreEqual(export.MetadataView.OptionValues.OrderBy(o => o.ToString()), "value1", "value2", "value3");
        }

        [TestMethod]
        public void TestMultipleStronglyTypedAttributesWithInvalidType()
        {
            var container = ContainerFactory.CreateWithDefaultAttributedCatalog();

            var export = container.GetExport<ExportMultiple, IMyOption2>();

            // IMyOption2 actually contains all the correct properties but just the wrong types so 
            // we can't actually get the metadata. 
            IMyOption2 metadata;
            ExceptionAssert.Throws<CompositionContractMismatchException>(() => metadata = export.MetadataView);
        }


#endif //!SILVERLIGHT

        #endregion


        [ExportMetadata("Name", "FromBaseType")]
        public abstract class BaseClassWithMetadataButNoExport
        {
        }

        [Export(typeof(BaseClassWithMetadataButNoExport))]
        public class DerivedClassWithExportButNoMetadata : BaseClassWithMetadataButNoExport
        {
        }

        [TestMethod]
        public void Metadata_BaseClassWithMetadataButNoExport()
        {
            var container = ContainerFactory.CreateWithAttributedCatalog(
                typeof(BaseClassWithMetadataButNoExport),
                typeof(DerivedClassWithExportButNoMetadata));

            var export = container.GetExport<BaseClassWithMetadataButNoExport>();

            Assert.IsFalse(export.Metadata.ContainsKey("Name"), "Export should only contain metadata from the derived!");
        }

        [Export(typeof(BaseClassWithExportButNoMetadata))]
        [PartExportsInherited]
        public abstract class BaseClassWithExportButNoMetadata
        {
        }


        [ExportMetadata("Name", "FromDerivedType")]
        public class DerivedClassMetadataButNoExport : BaseClassWithExportButNoMetadata
        {
        }

        [TestMethod]
        public void Metadata_BaseClassWithExportButNoMetadata()
        {
            var container = ContainerFactory.CreateWithAttributedCatalog(
                typeof(BaseClassWithExportButNoMetadata),
                typeof(DerivedClassMetadataButNoExport));

            var export = container.GetExport<BaseClassWithExportButNoMetadata>();

            Assert.IsFalse(export.Metadata.ContainsKey("Name"), "Export should only contain metadata from the base!");
        }

        [Export(typeof(BaseClassWithExportAndMetadata))]
        [ExportMetadata("Name", "FromBaseType")]
        public class BaseClassWithExportAndMetadata
        {
        }

        [Export(typeof(DerivedClassWithExportAndMetadata))]
        [ExportMetadata("Name", "FromDerivedType")]
        public class DerivedClassWithExportAndMetadata : BaseClassWithExportAndMetadata
        {
        }

        [TestMethod]
        public void Metadata_BaseAndDerivedWithExportAndMetadata()
        {
            var container = ContainerFactory.CreateWithAttributedCatalog(
                typeof(BaseClassWithExportAndMetadata),
                typeof(DerivedClassWithExportAndMetadata));

            var exportBase = container.GetExport<BaseClassWithExportAndMetadata>();

            Assert.AreEqual("FromBaseType", exportBase.Metadata["Name"]);

            var exportDerived = container.GetExport<DerivedClassWithExportAndMetadata>();
            Assert.AreEqual("FromDerivedType", exportDerived.Metadata["Name"]);
        }

        [Export]
        [ExportMetadata("Data", null, IsMultiple=true)]
        [ExportMetadata("Data", false, IsMultiple=true)]
        [ExportMetadata("Data", Int16.MaxValue, IsMultiple = true)]
        [ExportMetadata("Data", Int32.MaxValue, IsMultiple = true)]
        [ExportMetadata("Data", Int64.MaxValue, IsMultiple = true)]
        [ExportMetadata("Data", UInt16.MaxValue, IsMultiple = true)]
        [ExportMetadata("Data", UInt32.MaxValue, IsMultiple = true)]
        [ExportMetadata("Data", UInt64.MaxValue, IsMultiple = true)]
        [ExportMetadata("Data", "String", IsMultiple = true)]
        [ExportMetadata("Data", typeof(ClassWithLotsOfDifferentMetadataTypes), IsMultiple = true)]
        [ExportMetadata("Data", CreationPolicy.NonShared, IsMultiple=true)]
        [ExportMetadata("Data", new object[] { 1, 2, null }, IsMultiple=true)]
        [CLSCompliant(false)]
        public class ClassWithLotsOfDifferentMetadataTypes
        {
        }

        [TestMethod]
        public void ExportWithValidCollectionOfMetadata_ShouldDiscoverAllMetadata()
        {
            var catalog = CatalogFactory.CreateAttributed(typeof(ClassWithLotsOfDifferentMetadataTypes));

            var export = catalog.Parts.First().ExportDefinitions.First();

            var data = (object[])export.Metadata["Data"];

            Assert.AreEqual(12, data.Length);
        }

        [Export]
        [ExportMetadata("Data", null, IsMultiple = true)]
        [ExportMetadata("Data", 1, IsMultiple = true)]
        [ExportMetadata("Data", 2, IsMultiple = true)]
        [ExportMetadata("Data", 3, IsMultiple = true)]
        public class ClassWithIntCollectionWithNullValue
        {
        }

        [TestMethod]
        public void ExportWithIntCollectionPlusNullValueOfMetadata_ShouldDiscoverAllMetadata()
        {
            var catalog = CatalogFactory.CreateAttributed(typeof(ClassWithIntCollectionWithNullValue));

            var export = catalog.Parts.First().ExportDefinitions.First();

            var data = (object[])export.Metadata["Data"];

            Assert.IsNotInstanceOfType(data, typeof(int[]));

            Assert.AreEqual(4, data.Length);
        }

    }

    // Tests for metadata issues on export

    [Export]
    [ExportMetadata("foo", "bar1", IsMultiple = true)]
    [ExportMetadata("foo", "bar2", IsMultiple = true)]
    [ExportMetadata("acme", "acmebar", IsMultiple = true)]
    [ExportMetadata("acme", 2.0, IsMultiple = true)]
    [ExportMetadata("hello", "world")]
    [GoodStrongMetadata]
    public class MyExporterWithValidMetadata
    {
        [Export("ContractForValidMetadata")]
        [ExportMetadata("bar", "foo1", IsMultiple = true)]
        [ExportMetadata("bar", "foo2", IsMultiple = true)]
        [ExportMetadata("stuff", "acmebar", IsMultiple = true)]
        [ExportMetadata("stuff", 2.0, IsMultiple = true)]
        [ExportMetadata("world", "hello")] // the order of the attribute should not affect the result
        [GoodStrongMetadata]
        public double DoSomething() { return 0.618; }
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    [MetadataAttribute]
    public class GoodStrongMetadata : Attribute
    {
        public string GoodOne2 { get { return "GoodOneValue2"; } }
        public string ConflictedOne1 { get { return "ConflictedOneValue1"; } }
        public string ConflictedOne2 { get { return "ConflictedOneValue2"; } }
    }
    
    // Tests for metadata as part of contract

    public interface IMyExporter { }

    [Export]
    [Export(typeof(IMyExporter))]
    public class MyExporterWithNoMetadata : IMyExporter
    {
    }

    [Export]
    [Export(typeof(IMyExporter))]
    [ExportMetadata("Foo", "Bar")]
    public class MyExporterWithMetadata : IMyExporter
    {
    }


    public interface IMetadataFoo
    {
        string Foo { get; }
    }

    public interface IMetadataBar
    {
        string Bar { get; }
    }

    [Export]
    public class MyImporterWithExport
    {
        [Import(typeof(MyExporterWithNoMetadata))]
        public Export<MyExporterWithNoMetadata, IMetadataFoo> ValueInfo { get; set; }
    }

    [Export]
    public class SingleImportWithAllowDefault
    {
        [Import("Import", AllowDefault = true)]
        public Export Import { get; set; }
    }

    [Export]
    public class SingleImport
    {
        [Import("Import")]
        public Export Import { get; set; }
    }

    public interface IFooMetadataView
    {
        string Foo { get; }
    }

    [Export]
    public class MyImporterWithExportCollection
    {
        [ImportMany(typeof(MyExporterWithNoMetadata))]
        public IEnumerable<Export<MyExporterWithNoMetadata, IFooMetadataView>> ValueInfoCol { get; set; }
    }

    [Export]
    public class MyImporterWithExportForSelectiveImport
    {
        [Import]
        public Export<IMyExporter, IFooMetadataView> ValueInfo { get; set; }
    }

    [Export]
    public class MyImporterWithExportCollectionForSelectiveImport
    {
        [ImportMany]
        public ExportCollection<IMyExporter, IFooMetadataView> ValueInfoCol { get; set; }
    }

    public interface IMetadataView
    {
        string Foo { get; }

        [System.ComponentModel.DefaultValue(null)]
        string OptionalFoo { get; }
    }

    [Export]
    public class MyImporterWithExportStronglyTypedMetadata
    {
        [Import]
        public Export<IMyExporter, IMetadataView> ValueInfo { get; set; }
    }

    [Export]
    public class MyImporterWithExportCollectionStronglyTypedMetadata
    {
        [ImportMany]
        public ExportCollection<IMyExporter, IMetadataView> ValueInfoCol { get; set; }
    }

    public class MyExporterWithFullMetadata
    {
        [Export("MyStringContract")]
        public string String1 { get { return "String1"; } }

        [Export("MyStringContract")]
        [ExportMetadata("Foo", "fooValue")]
        public string String2 { get { return "String2"; } }

        [Export("MyStringContract")]
        [ExportMetadata("Bar", "barValue")]
        public string String3 { get { return "String3"; } }

        [Export("MyStringContract")]
        [ExportMetadata("Foo", "fooValue")]
        [ExportMetadata("Bar", "barValue")]
        public string String4 { get { return "String4"; } }
    }

    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class MyOption : Attribute
    {
        public MyOption(string name, object value)
        {
            OptionNames = name;
            OptionValues = value;
        }
        public string OptionNames { get; set; }
        public object OptionValues { get; set; }
    }

    public interface IMyOptions
    {
        IList<string> OptionNames { get; }
        ICollection<string> OptionValues { get; }
    }

    public interface IMyOptionsAsIEnumerable
    {
        IEnumerable<string> OptionNames { get; }
        IEnumerable<string> OptionValues { get; }
    }

    public interface IMyOptionsAsArray
    {
        string[] OptionNames { get; }
        string[] OptionValues { get; }
    }

    [Export]
    [MyOption("name1", "value1")]
    [MyOption("name2", "value2")]
    [ExportMetadata("OptionNames", "name3", IsMultiple = true)]
    [ExportMetadata("OptionValues", "value3", IsMultiple = true)]
    public class ExportMultiple
    {
    }

    public interface IMyOption2
    {
        string OptionNames { get; }
        string OptionValues { get; }
    }
}
