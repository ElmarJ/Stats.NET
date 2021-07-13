// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Factories;
using System.ComponentModel.Composition.Hosting;
using System.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.Composition.Primitives;

namespace System.ComponentModel.Composition
{
    [TestClass]
    public class ExportOfTTests
    {
        [TestMethod]
        public void Constructor1_ShouldNotThrow()
        {
            new NoOverridesExport<string>();
        }

        [TestMethod]
        public void Constructor2_NullAsExportedObjectGetterArgument_ShouldThrowArgumentNull()
        {
            ExceptionAssert.ThrowsArgument<ArgumentNullException>("exportedObjectGetter", () =>
            {
                new Export<string>((Func<string>)null);
            });
        }

        [TestMethod]
        public void Constructor3_NullAsExportedObjectGetterArgument_ShouldThrowArgumentNull()
        {
            var definition = ExportDefinitionFactory.Create();

            ExceptionAssert.ThrowsArgument<ArgumentNullException>("exportedObjectGetter", () =>
            {
                new Export<string>(definition, (Func<string>)null);
            });
        }

        [TestMethod]
        public void Constructor4_NullAsExportedObjectGetterArgument_ShouldThrowArgumentNull()
        {
            ExceptionAssert.ThrowsArgument<ArgumentNullException>("exportedObjectGetter", () =>
            {
                new Export<string>("ContractName", (Func<string>)null);
            });
        }

        [TestMethod]
        public void Constructor5_NullAsExportedObjectGetterArgument_ShouldThrowArgumentNull()
        {
            var metadata = new Dictionary<string, object>();

            ExceptionAssert.ThrowsArgument<ArgumentNullException>("exportedObjectGetter", () =>
            {
                new Export<string>("ContractName", metadata, (Func<string>)null);
            });
        }

        [TestMethod]
        public void Constructor3_NullAsDefinitionArgument_ShouldThrowArgumentNull()
        {
            ExceptionAssert.ThrowsArgument<ArgumentNullException>("definition", () =>
            {
                new Export<string>((ExportDefinition)null, () => null);
            });
        }

        [TestMethod]
        public void Constructor3_DefinitionAsDefinitionArgument_ShouldSetDefinitionProperty()
        {
            var definition = ExportDefinitionFactory.Create();

            var export = new Export<string>(definition, () => null);

            Assert.AreSame(definition, export.Definition);
        }

        [TestMethod]
        public void Constructor4_NullAsContractNameArgument_ShouldThrowArgumentNull()
        {
            ExceptionAssert.ThrowsArgument<ArgumentNullException>("contractName", () =>
            {
                new Export<string>((string)null, () => null);
            });
        }

        [TestMethod]
        public void Constructor5_NullAsContractNameArgument_ShouldThrowArgumentNull()
        {
            ExceptionAssert.ThrowsArgument<ArgumentNullException>("contractName", () =>
            {
                new Export<string>((string)null, new Dictionary<string, object>(), () => null);
            });
        }

        [TestMethod]
        public void Constructor4_EmptyStringAsContractNameArgument_ShouldThrowArgument()
        {
            ExceptionAssert.ThrowsArgument<ArgumentException>("contractName", () =>
            {
                new Export<string>(string.Empty, () => null);
            });
        }

        [TestMethod]
        public void Constructor5_EmptyStringAsContractNameArgument_ShouldThrowArgument()
        {
            ExceptionAssert.ThrowsArgument<ArgumentException>("contractName", () =>
            {
                new Export<string>(string.Empty, new Dictionary<string, object>(), () => null);
            });
        }

        [TestMethod]
        public void Constructor2_ValueAsT_ShouldSetDefinitionContractNameProperty()
        {
            var expectations = new List<Export>();
            expectations.Add(new Export<string>(() => null));
            expectations.Add(new Export<int>(() => 0));
            expectations.Add(new Export<int?>(() => null));
            expectations.Add(new Export<long>(() => 0));
            expectations.Add(new Export<Guid>(() => new Guid()));
            expectations.Add(new Export<Export>(() => null));

            foreach (var e in expectations)
            {
                Type type = e.GetType();

                string contractName = AttributedModelServices.GetContractName(type.GetGenericArguments()[0]);

                Assert.AreEqual(contractName, e.Definition.ContractName);
            }
        }

        [TestMethod]
        public void Constructor4_ValueAsContractNameArgument_ShouldSetDefinitionContractNameProperty()
        {
            var expectations = Expectations.GetContractNames();
            
            foreach (var e in expectations)
            {
                var export = new Export<string>(e, () => null);

                Assert.AreEqual(e, export.Definition.ContractName);
            }
        }

        [TestMethod]
        public void Constructor5_ValueAsContractNameArgument_ShouldSetDefinitionContractNameProperty()
        {
            var expectations = Expectations.GetContractNames();

            foreach (var e in expectations)
            {
                var export = new Export<string>(e, new Dictionary<string, object>(), () => null);

                Assert.AreEqual(e, export.Definition.ContractName);
            }
        }

        [TestMethod]
        public void Constructor2_ShouldSetMetadataPropertyToEmptyDictionary()
        {
            var export = new Export<string>(() => null); ;

            EnumerableAssert.IsEmpty(export.Metadata);
        }

        [TestMethod]
        public void Constructor4_ShouldSetMetadataPropertyToEmptyDictionary()
        {
            var export = new Export<string>("ContractName", () => null); ;

            EnumerableAssert.IsEmpty(export.Metadata);
        }

        [TestMethod]
        public void Constructor5_NullAsMetadataArgument_ShouldSetMetadataPropertyToEmptyDictionary()
        {
            var export = new Export<string>("ContractName", (IDictionary<string, object>)null, () => null); ;

            EnumerableAssert.IsEmpty(export.Metadata);
        }

        [TestMethod]
        public void Constructor2_ShouldSetMetadataPropertyToReadOnlyDictionary()
        {
            var export = new Export<string>(() => null);

            ExceptionAssert.Throws<NotSupportedException>(() =>
            {
                export.Metadata["Value"] = "Value";
            });
        }

        [TestMethod]
        public void Constructor4_NullAsMetadataArgument_ShouldSetMetadataPropertyToReadOnlyDictionary()
        {
            var export = new Export<string>("ContractName", () => null);

            ExceptionAssert.Throws<NotSupportedException>(() =>
            {
                export.Metadata["Value"] = "Value";
            });
        }

        [TestMethod]
        public void Constructor5_NullAsMetadataArgument_ShouldSetMetadataPropertyToReadOnlyDictionary()
        {
            var export = new Export<string>("ContractName", (IDictionary<string, object>)null, () => null);

            ExceptionAssert.Throws<NotSupportedException>(() =>
            {
                export.Metadata["Value"] = "Value";
            });
        }

        [TestMethod]
        public void Constructor5_WritableDictionaryAsMetadataArgument_ShouldSetMetadataPropertyToReadOnlyDictionary()
        {
            var export = new Export<string>("ContractName", new Dictionary<string, object>(), () => null);

            ExceptionAssert.Throws<NotSupportedException>(() =>
            {
                export.Metadata["Value"] = "Value";
            });
        }

        [TestMethod]
        public void Constructor5_DictionaryAsMetadataArgument_ShouldSetMetadataProperty()
        {
            var expectations = Expectations.GetMetadata();

            foreach (var e in expectations)
            {
                var export = new Export<string>("ContractName", e, () => null);

                EnumerableAssert.AreEqual(e, export.Metadata);
            }
        }

        [TestMethod]
        public void Constructor4_ShouldSetDefinitionMetadataPropertyToEmptyDictionary()
        {
            var export = new Export<string>("ContractName", () => null); ;

            EnumerableAssert.IsEmpty(export.Definition.Metadata);
        }

        [TestMethod]
        public void Constructor5_NullAsMetadataArgument_ShouldSetDefinitionMetadataPropertyToEmptyDictionary()
        {
            var export = new Export<string>("ContractName", (IDictionary<string, object>)null, () => null); ;

            EnumerableAssert.IsEmpty(export.Definition.Metadata);
        }

        [TestMethod]
        public void Constructor4_ShouldSetDefinitionMetadataPropertyToReadOnlyDictionary()
        {
            var export = new Export<string>("ContractName", () => null);

            ExceptionAssert.Throws<NotSupportedException>(() =>
            {
                export.Definition.Metadata["Value"] = "Value";
            });
        }

        [TestMethod]
        public void Constructor5_NullAsMetadataArgument_ShouldSetDefinitionMetadataPropertyToReadOnlyDictionary()
        {
            var export = new Export<string>("ContractName", (IDictionary<string, object>)null, () => null);

            ExceptionAssert.Throws<NotSupportedException>(() =>
            {
                export.Definition.Metadata["Value"] = "Value";
            });
        }

        [TestMethod]
        public void Constructor5_WritableDictionaryAsMetadataArgument_ShouldSetDefinitionMetadataPropertyToReadOnlyDictionary()
        {
            var export = new Export<string>("ContractName", new Dictionary<string, object>(), () => null);

            ExceptionAssert.Throws<NotSupportedException>(() =>
            {
                export.Definition.Metadata["Value"] = "Value";
            });
        }

        [TestMethod]
        public void Constructor5_DictionaryAsMetadataArgument_ShouldSetDefinitionMetadataProperty()
        {
            var expectations = Expectations.GetMetadata();

            foreach (var e in expectations)
            {
                var export = new Export<string>("ContractName", e, () => null);

                EnumerableAssert.AreEqual(e, export.Definition.Metadata);
            }
        }

        [TestMethod]
        public void Constructor2_FuncReturningAStringAsExportedObjectGetter_ShouldBeReturnedByGetExportedObject()
        {
            var export = new Export<string>(() => "Value");

            Assert.AreEqual("Value", export.GetExportedObject());
        }

        [TestMethod]
        public void Constructor3_FuncReturningAStringAsExportedObjectGetter_ShouldBeReturnedByGetExportedObject()
        {
            var definition = ExportDefinitionFactory.Create();

            var export = new Export<string>(definition, () => "Value");

            Assert.AreEqual("Value", export.GetExportedObject());
        }

        [TestMethod]
        public void Constructor4_FuncReturningAStringAsExportedObjectGetter_ShouldBeReturnedByGetExportedObject()
        {
            var export = new Export<string>("ContractName", () => "Value");

            Assert.AreEqual("Value", export.GetExportedObject());
        }

        [TestMethod]
        public void Constructor5_FuncReturningAStringAsExportedObjectGetter_ShouldBeReturnedByGetExportedObject()
        {
            var export = new Export<string>("ContractName", new Dictionary<string, object>(), () => "Value");

            Assert.AreEqual("Value", export.GetExportedObject());
        }

        [TestMethod]
        public void Constructor2_FuncReturningNullAsExportedObjectGetter_ShouldBeReturnedByGetExportedObject()
        {
            var export = new Export<string>(() => null);

            Assert.IsNull(export.GetExportedObject());
        }

        [TestMethod]
        public void Constructor3_FuncReturningNullAsExportedObjectGetter_ShouldBeReturnedByGetExportedObject()
        {
            var definition = ExportDefinitionFactory.Create();

            var export = new Export<string>(definition, () => null);

            Assert.IsNull(export.GetExportedObject());
        }

        [TestMethod]
        public void Constructor4_FuncReturningNullAsExportedObjectGetter_ShouldBeReturnedByGetExportedObject()
        {
            var export = new Export<string>("ContractName", () => null);

            Assert.IsNull(export.GetExportedObject());
        }

        [TestMethod]
        public void Constructor5_FuncReturningNullAsExportedObjectGetter_ShouldBeReturnedByGetExportedObject()
        {
            var export = new Export<string>("ContractName", new Dictionary<string, object>(), () => null);

            Assert.IsNull(export.GetExportedObject());
        }

        [TestMethod]
        public void Metadata_ShouldReturnOverriddenDefinitionMetadata()
        {
            var metadata = new Dictionary<string, object>();
            metadata["Name"] = "Value";

            var definition = ExportDefinitionFactory.Create("ContractName", metadata);

            var export = new DerivedExport<string>(definition);

            EnumerableAssert.AreEqual(metadata, export.Metadata);
        }

        [TestMethod]
        public void Definition_WhenNotOverridden_ShouldThrowNotImplemented()
        {
            var export = new NoOverridesExport<string>();

            ExceptionAssert.Throws<NotImplementedException>(() =>
            {
                var definition = export.Definition;
            });
        }

        [TestMethod]
        public void Metadata_WhenDefinitionNotOverridden_ShouldThrowNotImplemented()
        {
            var export = new NoOverridesExport<string>();

            ExceptionAssert.Throws<NotImplementedException>(() =>
            {
                var definition = export.Metadata;
            });
        }

        [TestMethod]
        public void GetExportedObject_ShouldReturnSameAsHiddenGetExportedObject()
        {
            var export = new DerivedExport<string>(() => "Value");

            Assert.AreEqual("Value", export.GetExportedObject());
            Assert.AreEqual("Value", ((Export)export).GetExportedObject());
        }

        [TestMethod]
        public void GetExportedObject_WhenGetExportedObjectCoreDoesNotReturnT_ShouldThrowContractMismatch()
        {
            var export = new DerivedExport<string>(() => 10);

            ExceptionAssert.Throws<CompositionContractMismatchException>(() =>
            {
                export.GetExportedObject();
            });
        }

        [TestMethod]
        public void GetExportedObjectHidden_WhenGetExportedObjectCoreDoesNotReturnT_ShouldThrowContractMismatch()
        {
            var export = new DerivedExport<string>(() => 10);

            ExceptionAssert.Throws<CompositionContractMismatchException>(() =>
            {
                ((Export)export).GetExportedObject();
            });
        }

        [TestMethod]
        public void GetExportedObject_WhenGetExportedObjectCoreNotOverridden_ShouldThrowNotImplemented()
        {
            var export = new NoOverridesExport<string>();

            ExceptionAssert.Throws<NotImplementedException>(() =>
            {
                export.GetExportedObject();
            });
        }

        [TestMethod]
        public void GetExportedObject_ShouldCacheExportedObjectGetter()
        {
            int count = 0;

            var export = new Export<int>("ContractName", () =>
                {
                    count++;
                    return count;
                });

            Assert.AreEqual(1, export.GetExportedObject());
            Assert.AreEqual(1, export.GetExportedObject());
            Assert.AreEqual(1, export.GetExportedObject());
        }

        [TestMethod]
        public void GetExportedObject_ShouldNotCacheOverrideGetExportedObjectCore()
        {
            int count = 0;

            var export = new DerivedExport<int>(() =>
            {
                count++;
                return count;
            });

            Assert.AreEqual(1, export.GetExportedObject());
            Assert.AreEqual(2, export.GetExportedObject());
            Assert.AreEqual(3, export.GetExportedObject());
        }

        private class NoOverridesExport<T> : Export<T>
        {
        }

        private class DerivedExport<T> : Export<T>
        {
            private readonly Func<object> _exportedObjectGetter;
            private readonly ExportDefinition _definition;

            public DerivedExport(ExportDefinition definition)
            {
                _definition = definition;
            }

            public DerivedExport(Func<T> exportedObjectGetter)
                : this(() => (object)exportedObjectGetter())
            {
            }

            public DerivedExport(Func<object> exportedObjectGetter)
            {
                _exportedObjectGetter = exportedObjectGetter;
            }

            public override ExportDefinition Definition
            {
                get { return _definition; }
            }

            protected override object GetExportedObjectCore()
            {
                return _exportedObjectGetter();
            }
        }
    }
}