// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition.Factories;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.UnitTesting;
using System.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.Composition
{
    [TestClass]
    public class ExportCollectionTests
    {
        [TestMethod]
        public void Constructor1_ShouldCreateEmptyCollection()
        {
            var collection = new ExportCollection();

            EnumerableAssert.IsEmpty(collection);
        }

        [TestMethod]
        public void Constructor2_NullAsItemsArgument_ShouldCreateEmptyCollection()
        {
            var collection = new ExportCollection((IEnumerable<Export>)null);

            EnumerableAssert.IsEmpty(collection);
        }

        [TestMethod]
        public void Constructor2_CollectionAsItemsArgument_ShouldPopulateCollection()
        {
            var exports = new Collection<Export>();
            exports.Add(new Export("Contract", null, () => null));
            exports.Add(new Export<string>(() => null));
            exports.Add(new Export<string, string>("Contract", null, () => null));
            exports.Add(new Export<string>(() => null));
            exports.Add(new Export<string, string>("Contract", null, () => null));
            exports.Add(new Export<string>(() => null));
            exports.Add(new Export<string, string>("Contract", null, () => null));

            var collection = new ExportCollection((IEnumerable<Export>)exports);

            EnumerableAssert.AreEqual(exports, collection);
        }

        [TestMethod]
        public void Constructor2_ArrayWithNullElementAsItemsArgument_ShouldPopulateCollectionWithNullItem()
        {
            var exports = new Export[1] { null };

            var collection = new ExportCollection(exports);

            Assert.AreEqual(1, collection.Count);
            Assert.IsNull(collection[0]);
        }

        public interface ICustomMetadata
        {
            bool PropertyName { get; }
        }

        public class Importer
        {
            [ImportMany("Value")]
            public ExportCollection CollectionPlain { get; set; }

            [ImportMany("EmptyValue")]
            public ExportCollection CollectionPlainEmpty { get; set; }

            [ImportMany("Value")]
            public ExportCollection<int> CollectionTyped { get; set; }

            [ImportMany("EmptyValue")]
            public ExportCollection<int> CollectionTypedEmpty { get; set; }

            [ImportMany("Value")]
            public ExportCollection<int, ICustomMetadata> CollectionTypedMetadata { get; set; }

            [ImportMany("EmptyValue")]
            public ExportCollection<int, ICustomMetadata> CollectionTypedMetadataEmpty { get; set; }

            [ImportMany("Value")]
            public IEnumerable<int> ReadWriteEnumerable { get; set; }

            [ImportMany("EmptyValue")]
            public IEnumerable<int> ReadWriteEnumerableEmpty { get; set; }

            [ImportMany("Value")]
            public IEnumerable<Export> MetadataUntypedEnumerable { get; set; }

            [ImportMany("EmptyValue")]
            public IEnumerable<Export> MetadataUntypedEnumerableEmpty { get; set; }

            [ImportMany("Value")]
            public IEnumerable<Export<int>> MetadataTypedEnumerable { get; set; }

            [ImportMany("EmptyValue")]
            public IEnumerable<Export<int>> MetadataTypedEnumerableEmpty { get; set; }

            [ImportMany("Value")]
            public IEnumerable<Export<int, ICustomMetadata>> MetadataFullyTypedEnumerable { get; set; }

            [ImportMany("EmptyValue")]
            public IEnumerable<Export<int, ICustomMetadata>> MetadataFullyTypedEnumerableEmpty { get; set; }

            public void VerifyImport(params int[] expectedValues)
            {
                ExportsAssert.AreEqual(CollectionPlain, expectedValues);
                EnumerableAssert.IsTrueForAll(CollectionPlain, i => true.Equals(i.Metadata["PropertyName"]));
                EnumerableAssert.IsEmpty(CollectionPlainEmpty);

                // Add a new Export to this collection to ensure that it doesn't
                // modifiy the other collections because they should each have there 
                // own collection instance
                CollectionPlain.Add(ExportFactory.Create("Name", "Value"));

                ExportsAssert.AreEqual(CollectionTyped, expectedValues);
                EnumerableAssert.IsTrueForAll(CollectionTyped, i => true.Equals(i.Metadata["PropertyName"]));
                EnumerableAssert.IsEmpty(CollectionTypedEmpty);

                ExportsAssert.AreEqual(CollectionTypedMetadata, expectedValues);
#if !SILVERLIGHT // Silverlight doesn't support strongly typed metadata
                EnumerableAssert.IsTrueForAll(CollectionTypedMetadata, i => true == i.MetadataView.PropertyName);
#endif //!SILVERLIGHT
                EnumerableAssert.IsEmpty(CollectionTypedMetadataEmpty);

                EnumerableAssert.AreEqual(ReadWriteEnumerable, expectedValues);
                EnumerableAssert.IsEmpty(ReadWriteEnumerableEmpty);

                ExportsAssert.AreEqual(MetadataUntypedEnumerable, expectedValues);
                EnumerableAssert.IsTrueForAll(MetadataUntypedEnumerable, i => true.Equals(i.Metadata["PropertyName"]));
                EnumerableAssert.IsEmpty(MetadataUntypedEnumerableEmpty);

                ExportsAssert.AreEqual(MetadataTypedEnumerable, expectedValues);
                EnumerableAssert.IsTrueForAll(MetadataTypedEnumerable, i => true.Equals(i.Metadata["PropertyName"]));
                EnumerableAssert.IsEmpty(MetadataTypedEnumerableEmpty);

                ExportsAssert.AreEqual(MetadataFullyTypedEnumerable, expectedValues);
#if !SILVERLIGHT // Silverlight doesn't support strongly typed metadata
                EnumerableAssert.IsTrueForAll(MetadataFullyTypedEnumerable, i => true == i.MetadataView.PropertyName);
#endif //!SILVERLIGHT
                EnumerableAssert.IsEmpty(MetadataFullyTypedEnumerableEmpty);
            }
        }

        public class ExporterDefault21
        {
            public ExporterDefault21() { Value = 21; }
            public ExporterDefault21(int v) { Value = v; }

            [Export("Value")]
            [ExportMetadata("PropertyName", true)]
            public int Value { get; set; }
        }

        public class ExporterDefault42
        {
            public ExporterDefault42() { Value = 42; }
            public ExporterDefault42(int v) { Value = v; }

            [Export("Value")]
            [ExportMetadata("PropertyName", true)]
            public int Value { get; set; }
        }


        [TestMethod]
        [TestProperty("Type", "Integration")]
        public void ImportCollectionsFormContainerOnly()
        {
            var container = ContainerFactory.Create();
            Importer importer = new Importer();

            CompositionBatch batch = new CompositionBatch();
            batch.AddParts(importer
                , new ExporterDefault21()
                , new ExporterDefault21(22)
                , new ExporterDefault42()
                , new ExporterDefault42(43));

            container.Compose(batch);

            importer.VerifyImport(21, 22, 42, 43);
        }

        [TestMethod]
        [TestProperty("Type", "Integration")]
        public void ImportCollectionsFromCatalogOnly()
        {
            var cat = CatalogFactory.CreateDefaultAttributed();
            var container = new CompositionContainer(cat);
            Importer importer = new Importer();

            CompositionBatch batch = new CompositionBatch();
            batch.AddParts(importer);
            container.Compose(batch);

            importer.VerifyImport(21, 42);
        }

        [TestMethod]
        [TestProperty("Type", "Integration")]
        public void ImportCollectionsFormContainerAndCatalog()
        {
            var cat = CatalogFactory.CreateDefaultAttributed();
            var container = new CompositionContainer(cat);
            Importer importer = new Importer();

            CompositionBatch batch = new CompositionBatch();
            batch.AddParts(importer
                , new ExporterDefault21(22)
                , new ExporterDefault42(43));

            container.Compose(batch);

            importer.VerifyImport(22, 43, 21, 42);
        }
    }
}
