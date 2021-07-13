// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.UnitTesting;

namespace System.ComponentModel.Composition
{
    [TestClass]
    public class ExportCollectionOfTTMetadataViewTests
    {
        [TestMethod]
        public void Constructor1_ShouldCreateEmptyCollection()
        {
            var collection = new ExportCollection<string, string>();

            EnumerableAssert.IsEmpty(collection);
        }

        [TestMethod]
        public void Constructor2_NullAsItemsArgument_ShouldCreateEmptyCollection()
        {
            var collection = new ExportCollection<string, string>((IEnumerable<Export<string, string>>)null);

            EnumerableAssert.IsEmpty(collection);
        }

        [TestMethod]
        public void Constructor2_CollectionAsItemsArgument_ShouldPopulateCollection()
        {
            Collection<Export<string, string>> exports = new Collection<Export<string, string>>();
            exports.Add(new Export<string, string>("Contract", null, () => null));
            exports.Add(new Export<string, string>("Contract", null, () => null));
            exports.Add(new Export<string, string>("Contract", null, () => null));

            var collection = new ExportCollection<string, string>((IEnumerable<Export<string, string>>)exports);

            EnumerableAssert.AreEqual(exports, collection);
        }

        [TestMethod]
        public void Constructor2_ArrayWithNullElementAsItemsArgument_ShouldPropulateCollectionWithNullItem()
        {
            var exports = new Export<string, string>[1] { null };

            var collection = new ExportCollection<string, string>(exports);

            Assert.AreEqual(1, collection.Count);
            Assert.IsNull(collection[0]);
        }
    }
}