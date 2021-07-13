//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System;
using System.ComponentModel.Composition;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq.Expressions;
using System.UnitTesting;
using System.ComponentModel.Composition.Factories;
using System.ComponentModel.Composition.UnitTesting;
using System.ComponentModel.Composition.Primitives;

namespace System.ComponentModel.Composition
{
    [TestClass]
    public class ContractBasedImportDefinitionTests
    {
        [TestMethod]
        public void Constructor1_ShouldSetRequiredMetadataPropertyToEmptyEnumerable()
        {
            var definition = new NoOverridesContractBasedImportDefinition();

            EnumerableAssert.IsEmpty(definition.RequiredMetadata);
        }

        [TestMethod]
        public void Constructor1_ShouldSetCardinalityPropertyToExactlyOne()
        {
            var definition = new NoOverridesContractBasedImportDefinition();

            Assert.AreEqual(ImportCardinality.ExactlyOne, definition.Cardinality);
        }

        [TestMethod]
        public void Constructor1_ShouldSetIsPrerequisitePropertyToTrue()
        {
            var definition = new NoOverridesContractBasedImportDefinition();

            Assert.IsTrue(definition.IsPrerequisite);
        }

        [TestMethod]
        public void Constructor1_ShouldSetIsRecomposablePropertyToFalse()
        {
            var definition = new NoOverridesContractBasedImportDefinition();

            Assert.IsFalse(definition.IsRecomposable);
        }

        [TestMethod]
        public void Constructor1_ShouldSetRequiredCreationPolicyToAny()
        {
            var definition = new NoOverridesContractBasedImportDefinition();

            Assert.AreEqual(CreationPolicy.Any, definition.RequiredCreationPolicy);
        }

        [TestMethod]
        public void Constructor2_NullAsContractNameArgument_ShouldThrowArgumentNull()
        {
            ExceptionAssert.ThrowsArgument<ArgumentNullException>("contractName", () =>
            {
                new ContractBasedImportDefinition((string)null, (string)null, Enumerable.Empty<string>(), ImportCardinality.ExactlyOne, false, false, CreationPolicy.Any);
            });
        }

        [TestMethod]
        public void Constructor2_EmptyStringAsContractNameArgument_ShouldThrowArgument()
        {
            ExceptionAssert.ThrowsArgument<ArgumentException>("contractName", () =>
            {
                new ContractBasedImportDefinition("", (string)null, Enumerable.Empty<string>(), ImportCardinality.ExactlyOne, false, false, CreationPolicy.Any);
            });
        }

        [TestMethod]
        public void Constructor2_ArrayWithNullElementAsRequiredMetadataArgument_ShouldThrowArgument()
        {
            var requiredMetadata = new string[] { null };

            ExceptionAssert.ThrowsArgument<ArgumentException>("requiredMetadata", () =>
            {
                new ContractBasedImportDefinition("requiredMetadata", (string)null, requiredMetadata, ImportCardinality.ExactlyOne, false, false, CreationPolicy.Any);
            });
        }

        [TestMethod]
        public void Constructor2_NullAsRequiredMetadataArgument_ShouldSetRequiredMetadataToEmptyEnumerable()
        {
            var definition = new ContractBasedImportDefinition("requiredMetadata", (string)null, (IEnumerable<string>)null, ImportCardinality.ExactlyOne, false, false, CreationPolicy.Any);

            EnumerableAssert.IsEmpty(definition.RequiredMetadata);
        }

        [TestMethod]
        public void Constructor2_OutOfRangeValueAsCardinalityArgument_ShouldThrowArgument()
        {
            var expectations = Expectations.GetInvalidEnumValues<ImportCardinality>();

            foreach (var e in expectations)
            {
                ExceptionAssert.ThrowsArgument<ArgumentException>("cardinality", () =>
                {
                    new ContractBasedImportDefinition((string)null, (string)null, Enumerable.Empty<string>(), e, false, false, CreationPolicy.Any);
                });
            }
        }

        [TestMethod]
        public void Constructor2_ValueAsCardinalityArgument_ShouldSetCardinalityProperty()
        {
            var expectations = Expectations.GetEnumValues<ImportCardinality>();

            foreach (var e in expectations)
            {
                var definition = new ContractBasedImportDefinition("ContractName", (string)null, Enumerable.Empty<string>(), e, false, false, CreationPolicy.Any);

                Assert.AreEqual(e, definition.Cardinality);
            }
        }

        [TestMethod]
        public void Constructor2_ValueAsContractNameArgument_ShouldSetContractNameProperty()
        {
            var expectations = Expectations.GetContractNames();

            foreach (var e in expectations)
            {
                var definition = new ContractBasedImportDefinition(e, (string)null, Enumerable.Empty<string>(), ImportCardinality.ExactlyOne, false, false, CreationPolicy.Any);

                Assert.AreEqual(e, definition.ContractName);
            }
        }

        [TestMethod]
        public void Constructor2_ValueAsRequiredMetadataArgument_ShouldSetRequiredMetadataProperty()
        {
            var expectations = Expectations.GetRequiredMetadataWithEmpty();

            foreach (var e in expectations)
            {
                var definition = new ContractBasedImportDefinition("ContractName", (string)null, e, ImportCardinality.ExactlyOne, false, false, CreationPolicy.Any);

                EnumerableAssert.AreEqual(e, definition.RequiredMetadata);
            }
        }

        [TestMethod]
        public void Constructor2_ValueAsIsRecomposableArgument_ShouldSetIsRecomposableProperty()
        {
            var expectations = Expectations.GetBooleans();

            foreach (var e in expectations)
            {
                var definition = new ContractBasedImportDefinition("ContractName", (string)null, Enumerable.Empty<string>(), ImportCardinality.ExactlyOne, e, false, CreationPolicy.Any);

                Assert.AreEqual(e, definition.IsRecomposable);
            }
        }

        [TestMethod]
        public void Constructor2_ValueAsIsPrerequisiteArgument_ShouldSetIsPrerequisiteProperty()
        {
            var expectations = Expectations.GetBooleans();

            foreach (var e in expectations)
            {
                var definition = new ContractBasedImportDefinition("ContractName", (string)null, Enumerable.Empty<string>(), ImportCardinality.ExactlyOne, false, e, CreationPolicy.Any);

                Assert.AreEqual(e, definition.IsPrerequisite);
            }
        }

        [TestMethod]
        public void Constructor2_ShouldSetRequiredCreationPolicyToAny()
        {
            var expectations = Expectations.GetEnumValues<CreationPolicy>();

            foreach (var e in expectations)
            {
                var definition = new ContractBasedImportDefinition("ContractName", (string)null, Enumerable.Empty<string>(), ImportCardinality.ExactlyOne, false, false, e);

                Assert.AreEqual(e, definition.RequiredCreationPolicy);
            }
        }

        [TestMethod]
        public void ContractName_WhenNotOverridden_ShouldThrowNotImplemented()
        {
            var definition = new NoOverridesContractBasedImportDefinition();

            ExceptionAssert.Throws<NotImplementedException>(() =>
            {
                var contractName = definition.ContractName;
            });
        }

        [TestMethod]
        public void Constraint_WhenContractNameNotOverridden_ShouldThrowNotImplemented()
        {
            var definition = new NoOverridesContractBasedImportDefinition();

            ExceptionAssert.Throws<NotImplementedException>(() =>
            {
                var constraint = definition.Constraint;
            });
        }

        [TestMethod]
        public void Constraint_ShouldIncludeContractNameProperty()
        {
            var expectations = Expectations.GetContractNames();

            foreach (var e in expectations)
            {
                var definition = new ContractBasedImportDefinition(e, (string)null, (IEnumerable<string>)null, ImportCardinality.ExactlyOne, true, false, CreationPolicy.Any);

                ConstraintAssert.Contains(definition.Constraint, e);
            }
        }

        [TestMethod]
        public void Constraint_ShouldIncludeRequiredMetadataProperty()
        {
            var expectations = Expectations.GetRequiredMetadataWithEmpty();

            foreach (var e in expectations)
            {
                var definition = new ContractBasedImportDefinition("ContractName", (string)null, e, ImportCardinality.ExactlyOne, true, false, CreationPolicy.Any);

                ConstraintAssert.Contains(definition.Constraint, "ContractName", e);
            }
        }

        [TestMethod]
        public void Constraint_ShouldIncludeOverriddenContractNameProperty()
        {
            var expectations = Expectations.GetContractNames();

            foreach (var e in expectations)
            {
                var definition = new DerivedContractBasedImportDefinition(e);

                ConstraintAssert.Contains(definition.Constraint, e);
            }
        }

        [TestMethod]
        public void Constraint_ShouldIncludeOverriddenRequiredMetadata()
        {
            var expectations = Expectations.GetRequiredMetadataWithEmpty();

            foreach (var e in expectations)
            {
                var definition = new DerivedContractBasedImportDefinition("ContractName", e);

                ConstraintAssert.Contains(definition.Constraint, "ContractName", e);
            }
        }

        private class NoOverridesContractBasedImportDefinition : ContractBasedImportDefinition
        {
            public NoOverridesContractBasedImportDefinition()
            {
            }
        }

        private class DerivedContractBasedImportDefinition : ContractBasedImportDefinition
        {
            private readonly string _contractName;
            private readonly IEnumerable<string> _requiredMetadata;

            public DerivedContractBasedImportDefinition(string contractName)
            {
                _contractName = contractName;
            }

            public DerivedContractBasedImportDefinition(string contractName, IEnumerable<string> requiredMetadata)
            {
                _contractName = contractName;
                _requiredMetadata = requiredMetadata;
            }

            public override string ContractName
            {
                get { return _contractName; }
            }

            public override IEnumerable<string> RequiredMetadata
            {
                get { return _requiredMetadata; }
            }
        }
    }
}

