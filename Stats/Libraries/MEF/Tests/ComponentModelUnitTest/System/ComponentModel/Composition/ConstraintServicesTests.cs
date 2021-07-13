// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Linq.Expressions;
using System.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.Composition
{
    [TestClass]
    public class ConstraintServicesTests
    {
        [TestMethod]
        public void TypeIdentityConstraint_ValidMatchingExportDef_ShouldMatch()
        {
            var contractName = "MyContract";
            var typeIdentity = AttributedModelServices.GetTypeIdentity(typeof(ConstraintServicesTests));
            var metadata = new Dictionary<string, object>();
            metadata.Add(CompositionConstants.ExportTypeIdentityMetadataName, typeIdentity);

            var exportDefinition = new ExportDefinition(contractName, metadata);

            var constraint = ConstraintServices.CreateConstraint(contractName, typeIdentity, null, CreationPolicy.Any);

            var predicate = constraint.Compile();

            Assert.IsTrue(predicate(exportDefinition));
        }

        [TestMethod]
        public void TypeIdentityConstraint_ValidNonMatchingExportDef_ShouldNotMatch()
        {
            var contractName = "MyContract";
            var typeIdentity = AttributedModelServices.GetTypeIdentity(typeof(ConstraintServicesTests));
            var metadata = new Dictionary<string, object>();
            metadata.Add(CompositionConstants.ExportTypeIdentityMetadataName, typeIdentity + "Another Identity");

            var exportDefinition = new ExportDefinition(contractName, metadata);

            var constraint = ConstraintServices.CreateConstraint(contractName, typeIdentity, null, CreationPolicy.Any);

            var predicate = constraint.Compile();

            Assert.IsFalse(predicate(exportDefinition));
        }

        [TestMethod]
        public void TypeIdentityConstraint_InvalidExportDef_ShouldNotMatch()
        {
            var contractName = "MyContract";
            var typeIdentity = AttributedModelServices.GetTypeIdentity(typeof(ConstraintServicesTests));
            var metadata = new Dictionary<string, object>();

            var exportDefinition = new ExportDefinition(contractName, metadata);

            var constraint = ConstraintServices.CreateConstraint(contractName, typeIdentity, null, CreationPolicy.Any);

            var predicate = constraint.Compile();

            Assert.IsFalse(predicate(exportDefinition));
        }

        [TestMethod]
        public void CreationPolicyConstraint_ValidMatchingCreationPolicy_ShouldMatch()
        {
            var contractName = "MyContract";
            var metadata = new Dictionary<string, object>();
            metadata.Add(CompositionConstants.PartCreationPolicyMetadataName, CreationPolicy.Shared);

            var exportDefinition = new ExportDefinition(contractName, metadata);

            var constraint = ConstraintServices.CreateConstraint(contractName, null, null, CreationPolicy.Shared);

            var predicate = constraint.Compile();

            Assert.IsTrue(predicate(exportDefinition));
        }

        [TestMethod]
        public void CreationPolicyConstraint_ValidNonMatchingCreationPolicy_ShouldNotMatch()
        {
            var contractName = "MyContract";
            var metadata = new Dictionary<string, object>();
            metadata.Add(CompositionConstants.PartCreationPolicyMetadataName, CreationPolicy.NonShared);

            var exportDefinition = new ExportDefinition(contractName, metadata);

            var constraint = ConstraintServices.CreateConstraint(contractName, null, null, CreationPolicy.Shared);

            var predicate = constraint.Compile();

            Assert.IsFalse(predicate(exportDefinition));
        }

        [TestMethod]
        public void CreationPolicyConstraint_InvalidCreationPolicy_ShouldNotMatch()
        {
            var contractName = "MyContract";
            var metadata = new Dictionary<string, object>();
            metadata.Add(CompositionConstants.PartCreationPolicyMetadataName, "Shared");

            var exportDefinition = new ExportDefinition(contractName, metadata);

            var constraint = ConstraintServices.CreateConstraint(contractName, null, null, CreationPolicy.Shared);

            var predicate = constraint.Compile();

            Assert.IsFalse(predicate(exportDefinition));
        }

        [TestMethod]
        public void CreationPolicyConstraint_NoCreationPolicy_ShouldNotMatch()
        {
            var contractName = "MyContract";
            var metadata = new Dictionary<string, object>();

            var exportDefinition = new ExportDefinition(contractName, metadata);

            var constraint = ConstraintServices.CreateConstraint(contractName, null, null, CreationPolicy.Shared);

            var predicate = constraint.Compile();

            Assert.IsTrue(predicate(exportDefinition));
        }

        [TestMethod]
        public void TryParseConstraint_ConstraintFromCreateConstraintAsConstraintArgument1_CanParse()
        {
            var expectations = Expectations.GetContractNamesWithEmpty();
            
            foreach (var e in expectations)
            {
                var constraint = ConstraintServices.CreateConstraint((string)e, null, null, CreationPolicy.Any);

                AssertCanParse(constraint, e);
            }
        }

        [TestMethod]
        public void TryParseConstraint_ConstraintFromCreateConstraintAsConstraintArgument2_CanParse()
        {
            var expectations = Expectations.GetRequiredMetadata();
            
            foreach (var e in expectations)
            {
                var constraint = ConstraintServices.CreateConstraint((IEnumerable<string>)e);

                AssertCanParse(constraint, (string)null, e);
            }
        }

        [TestMethod]
        public void TryParseConstraint_ConstraintFromCreateConstraintAsConstraintArgument3_CanParse()
        {
            var contractNames = Expectations.GetContractNames();
            var metadataValues = Expectations.GetRequiredMetadata();
            
            foreach (var contractName in contractNames)
            {
                foreach (var metadataValue in metadataValues)
                {
                    var constraint = ConstraintServices.CreateConstraint(contractName, null, metadataValue, CreationPolicy.Any);

                    AssertCanParse(constraint, contractName, metadataValue);
                }
            }
        }

        [TestMethod]
        public void TryParseConstraint_ContractNameOperatorEqualsAsConstraintArgument_CanParse()
        {
            var expectations = new ExpectationCollection<Expression<Func<ExportDefinition, bool>>, string>();
            expectations.Add(item => item.ContractName == "", "");
            expectations.Add(item => item.ContractName == " ", " ");
            expectations.Add(item => item.ContractName == "   ", "   ");
            expectations.Add(item => item.ContractName == "ContractName", "ContractName");
            expectations.Add(item => item.ContractName == "contractName", "contractName");
            expectations.Add(item => item.ContractName == "{ContractName}", "{ContractName}");
            expectations.Add(item => item.ContractName == "{ContractName}Name", "{ContractName}Name");
            expectations.Add(item => item.ContractName == "System.Windows.Forms.Control", "System.Windows.Forms.Control");
            expectations.Add(item => item.ContractName == "{System.Windows.Forms}Control", "{System.Windows.Forms}Control");
            
            foreach (var e in expectations)
            {
                AssertCanParse(e.Input, e.Output);
            }
        }

        [TestMethod]
        public void TryParseConstraint_MetadataContainsKeyAsConstraintArgument_CanParse()
        {
            var expectations = new ExpectationCollection<Expression<Func<ExportDefinition, bool>>, string[]>();
            expectations.Add(item => item.Metadata.ContainsKey(""), new string[] { "" });
            expectations.Add(item => item.Metadata.ContainsKey("Value"), new string[] { "Value" });
            expectations.Add(item => item.Metadata.ContainsKey("value"), new string[] { "value" });
            expectations.Add(item => item.Metadata.ContainsKey("Value") && item.Metadata.ContainsKey("value"), new string[] { "Value", "value" });
            expectations.Add(item => item.Metadata.ContainsKey("Value") && item.Metadata.ContainsKey("value") && item.Metadata.ContainsKey("Metadata"), new string[] { "Value", "value", "Metadata" });

            foreach (var e in expectations)
            {
                AssertCanParse(e.Input, (string)null, e.Output);
            }
        }

        [TestMethod]
        public void TryParseConstraint_ContractNameOperatorEqualsAndMetadataContainsKeyAsConstraintArgument_CanParse()
        {
            var expectations = new ExpectationCollection<Expression<Func<ExportDefinition, bool>>, KeyValuePair<string, string[]>>();
            expectations.Add(item => item.ContractName == "ContractName" && item.Metadata.ContainsKey(""), new KeyValuePair<string, string[]>("ContractName", new string[] { "" }));
            expectations.Add(item => item.ContractName == "ContractName" && item.Metadata.ContainsKey("Value"), new KeyValuePair<string, string[]>("ContractName", new string[] { "Value" }));
            expectations.Add(item => item.Metadata.ContainsKey("Value") && item.ContractName == "ContractName", new KeyValuePair<string, string[]>("ContractName", new string[] { "Value" }));
            expectations.Add(item => item.Metadata.ContainsKey("Value") && item.ContractName == "ContractName" && item.Metadata.ContainsKey("value"), new KeyValuePair<string, string[]>("ContractName", new string[] { "Value", "value" }));
            expectations.Add(item => item.ContractName == "ContractName" && item.Metadata.ContainsKey("value"), new KeyValuePair<string, string[]>("ContractName", new string[] { "value" }));
            expectations.Add(item => item.ContractName == "ContractName" && item.Metadata.ContainsKey("Value") && item.Metadata.ContainsKey("value"), new KeyValuePair<string, string[]>("ContractName", new string[] { "Value", "value" }));
            expectations.Add(item => item.ContractName == "ContractName" && item.Metadata.ContainsKey("Value") && item.Metadata.ContainsKey("value") && item.Metadata.ContainsKey("Metadata"), new KeyValuePair<string, string[]>("ContractName", new string[] { "Value", "value", "Metadata" }));

            foreach (var e in expectations)
            {
                AssertCanParse(e.Input, e.Output.Key, e.Output.Value);
            }
        }


        [TestMethod]
        public void TryParseConstraint_ContractNameReverseOperatorEqualsAsConstraintArgument_CanParse()
        {
            var expectations = new ExpectationCollection<Expression<Func<ExportDefinition, bool>>, string>();
            expectations.Add(item => "" == item.ContractName, "");
            expectations.Add(item => " " == item.ContractName, " ");
            expectations.Add(item => "   " == item.ContractName, "   ");
            expectations.Add(item => "ContractName" == item.ContractName, "ContractName");
            expectations.Add(item => "contractName" == item.ContractName, "contractName");
            expectations.Add(item => "{ContractName}" == item.ContractName, "{ContractName}");
            expectations.Add(item => "{ContractName}Name" == item.ContractName, "{ContractName}Name");
            expectations.Add(item => "System.Windows.Forms.Control" == item.ContractName, "System.Windows.Forms.Control");
            expectations.Add(item => "{System.Windows.Forms}Control" == item.ContractName , "{System.Windows.Forms}Control");

            foreach (var e in expectations)
            {
                AssertCanParse(e.Input, e.Output);
            }
        }

        [TestMethod]
        public void TryParseConstraint_ContractNameEqualsAsConstraintArgument_CanNotParse()
        {
            var expectations = new List<Expression<Func<ExportDefinition, bool>>>();
            expectations.Add(item => item.ContractName == null);
            expectations.Add(item => item.ContractName == string.Empty);
            expectations.Add(item => item.ToString() == "Value");
            expectations.Add(item => item.ContractName != "ContractName");
            expectations.Add(item => item.ContractName == "ContractName" || item.ContractName == "ContractName");
            expectations.Add(item => item.ContractName == "ContractName" && item.ContractName == "ContractName");
            expectations.Add(item => item.ContractName.Equals("ContractName"));
            expectations.Add(item => !(item.ContractName == "ContractName"));
            expectations.Add(item => "ContractName" != item.ContractName);
            expectations.Add(item => "ContractName".Equals(item.ContractName));
            
            foreach (var e in expectations)
            {
                AssertCanNotParse(e);
            }
        }

        [TestMethod]
        public void TryParseConstraint_IncorrectFormedMetadataContainsKeyAsConstraintArgument_CanParse()
        {
            var value = new { Metadata = (IDictionary<string, object>)new Dictionary<string, object>() };

            var expectations = new List<Expression<Func<ExportDefinition, bool>>>();
            expectations.Add(item => true);
            expectations.Add(item => item.Metadata.Count == 0);
            expectations.Add(item => item.Metadata.Remove("Value"));
            expectations.Add(item => ((IDictionary<string, object>)(new Dictionary<string, object>())).ContainsKey("Value"));
            expectations.Add(item => value.Metadata.ContainsKey("Value"));
            expectations.Add(item => item.Metadata.ContainsKey(null));
            expectations.Add(item => item.Metadata.ContainsKey(string.Empty));
            expectations.Add(item => !item.Metadata.ContainsKey("Value"));
            expectations.Add(item => item.Metadata.ContainsKey("Value") || item.Metadata.ContainsKey("value"));
            expectations.Add(item => item.Metadata.ContainsKey("Value") && (!item.Metadata.ContainsKey("value")));
            expectations.Add(item => item.Metadata.ContainsKey("Value") && item.Metadata.ContainsKey("value") || item.Metadata.ContainsKey("value"));

            foreach (var e in expectations)
            {
                AssertCanNotParse(e);
            }
        }

        private static void AssertCanParse(Expression<Func<ExportDefinition, bool>> constraint, string contractName, params string[] requiredMetadata)
        {
            Assert.IsNotNull(constraint);

            string contractNameResult = null;
            IEnumerable<string> requiredMetadataResult = null;
            bool success = ContraintParser.TryParseConstraint(constraint, out contractNameResult, out requiredMetadataResult);

            Assert.IsTrue(success);
            Assert.AreEqual(contractName, contractNameResult);
            EnumerableAssert.AreEqual(requiredMetadata, requiredMetadataResult);
        }

        private static void AssertCanNotParse(Expression<Func<ExportDefinition, bool>> constraint)
        {
            Assert.IsNotNull(constraint);

            string contractNameResult;
            IEnumerable<string> requiredMetadataResult;

            var success = ContraintParser.TryParseConstraint(constraint, out contractNameResult, out requiredMetadataResult);
            Assert.IsFalse(success);
            Assert.IsNull(contractNameResult);
            Assert.IsNull(requiredMetadataResult);
        }
    }
}
