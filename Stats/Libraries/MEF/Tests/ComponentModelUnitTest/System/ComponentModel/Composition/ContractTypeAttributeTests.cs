// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.UnitTesting;

namespace System.ComponentModel.Composition
{
    [TestClass]
    public class ContractTypeAttributeTests
    {
        [TestMethod]
        public void Constructor1_ShouldSetContractNamePropertyToEmptyString()
        {
            var attribute = new ContractTypeAttribute();

            Assert.AreEqual(string.Empty, attribute.ContractName);
        }

        [TestMethod]
        public void Constructor2_NullAsContractNameArgument_ShouldSetContractNamePropertyToEmptyString()
        {
            var attribute = new ContractTypeAttribute((string)null);

            Assert.AreEqual(string.Empty, attribute.ContractName);
        }

        [TestMethod]
        public void Constructor2_ValueAsContractNameArgument_ShouldSetContractNameProperty()
        {
            var expectations = Expectations.GetContractNamesWithEmpty();
            
            foreach (var e in expectations)
            {
                var a = new ContractTypeAttribute(e);

                Assert.AreEqual(e, a.ContractName);
            }
        }

        [TestMethod]
        public void Constructor1_ShouldSetMetadataViewTypePropertyToNull()
        {
            var attribute = new ContractTypeAttribute();

            Assert.IsNull(attribute.MetadataViewType);
        }

        [TestMethod]
        public void Constructor2_ShouldSetMetadataViewTypePropertyToNull()
        {
            var attribute = new ContractTypeAttribute("Contract");

            Assert.IsNull(attribute.MetadataViewType);
        }

        [TestMethod]
        public void MetadataViewType_NullAsValueArgument_ShouldSetPropertyToNull()
        {
            var attribute = new ContractTypeAttribute();
            attribute.MetadataViewType = null;

            Assert.IsNull(attribute.MetadataViewType);
        }

        [TestMethod]
        public void MetadataViewType_ValueAsValueArgument_ShouldSetProperty()
        {
            var expectations = Expectations.GetTypes();
           
            foreach (var e in expectations)
            {
                var attribute = new ContractTypeAttribute();
                attribute.MetadataViewType = e;

                Assert.AreEqual(e, attribute.MetadataViewType);
            }            
        }
    }
}
