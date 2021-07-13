// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Factories;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.Composition
{
    [TestClass]
    public class ExportOfTTMetadataTests
    {
        [TestMethod]
        public void Constructor1_ShouldNotThrow()
        {
            new NoOverridesExport<string, string>();
        }

        [TestMethod]
        public void Constructor2_NullAsExportedObjectGetterArgument_ShouldThrowArgumentNull()
        {
            var definition = ExportDefinitionFactory.Create();

            ExceptionAssert.ThrowsArgument<ArgumentNullException>("exportedObjectGetter", () =>
            {
                new Export<string, string>(definition, (Func<string>)null);
            });
        }

        [TestMethod]
        public void Constructor3_NullAsExportedObjectGetterArgument_ShouldThrowArgumentNull()
        {
            var metadata = new Dictionary<string, object>();

            ExceptionAssert.ThrowsArgument<ArgumentNullException>("exportedObjectGetter", () =>
            {
                new Export<string, string>(metadata, (Func<string>)null);
            });
        }

        [TestMethod]
        public void Constructor4_NullAsExportedObjectGetterArgument_ShouldThrowArgumentNull()
        {
            var metadata = new Dictionary<string, object>();

            ExceptionAssert.ThrowsArgument<ArgumentNullException>("exportedObjectGetter", () =>
            {
                new Export<string, string>("ContractName", metadata, (Func<string>)null);
            });
        }

        [TestMethod]
        public void Constructor2_NullAsDefinitionArgument_ShouldThrowArgumentNull()
        {
            ExceptionAssert.ThrowsArgument<ArgumentNullException>("definition", () =>
            {
                new Export<string, string>((ExportDefinition)null, () => null);
            });
        }

        [TestMethod]
        public void Constructor2_DefinitionAsDefinitionArgument_ShouldSetDefinitionProperty()
        {
            var definition = ExportDefinitionFactory.Create();

            var export = new Export<string, string>(definition, () => null);

            Assert.AreSame(definition, export.Definition);
        }

        [TestMethod]
        public void Constructor4_NullAsContractNameArgument_ShouldThrowArgumentNull()
        {
            ExceptionAssert.ThrowsArgument<ArgumentNullException>("contractName", () =>
            {
                new Export<string, string>((string)null, new Dictionary<string, object>(), () => null);
            });
        }

        [TestMethod]
        public void Constructor4_EmptyStringAsContractNameArgument_ShouldThrowArgument()
        {
            ExceptionAssert.ThrowsArgument<ArgumentException>("contractName", () =>
            {
                new Export<string, string>(string.Empty, new Dictionary<string, object>(), () => null);
            });
        }

        [TestMethod]
        public void Constructor3_ValueAsT_ShouldSetDefinitionContractNameProperty()
        {
            var metadata = new Dictionary<string, object>();

            var expectations = new List<Export>();
            expectations.Add(new Export<string, string>(metadata, () => null));
            expectations.Add(new Export<int, string>(metadata, () => 0));
            expectations.Add(new Export<int?, string>(metadata, () => null));
            expectations.Add(new Export<long, string>(metadata, () => 0));
            expectations.Add(new Export<Guid, string>(metadata, () => new Guid()));
            expectations.Add(new Export<Export, string>(metadata, () => null));

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
                var export = new Export<string, string>(e, new Dictionary<string, object>(), () => null);

                Assert.AreEqual(e, export.Definition.ContractName);
            }
        }

        [TestMethod]
        public void Constructor3_NullAsMetadataArgument_ShouldSetMetadataPropertyToEmptyDictionary()
        {
            var export = new Export<string, string>((IDictionary<string, object>)null, () => null); ;

            EnumerableAssert.IsEmpty(export.Metadata);
        }

        [TestMethod]
        public void Constructor3_NullAsMetadataArgument_ShouldSetMetadataPropertyToReadOnlyDictionary()
        {
            var export = new Export<string, string>((IDictionary<string, object>)null, () => null);

            ExceptionAssert.Throws<NotSupportedException>(() =>
            {
                export.Metadata["Value"] = "Value";
            });
        }

        [TestMethod]
        public void Constructor3_WritableDictionaryAsMetadataArgument_ShouldSetMetadataPropertyToReadOnlyDictionary()
        {
            var export = new Export<string, string>(new Dictionary<string, object>(), () => null);

            ExceptionAssert.Throws<NotSupportedException>(() =>
            {
                export.Metadata["Value"] = "Value";
            });
        }

        [TestMethod]
        public void Constructor3_DictionaryAsMetadataArgument_ShouldSetMetadataProperty()
        {
            var expectations = Expectations.GetMetadata();

            foreach (var e in expectations)
            {
                var export = new Export<string, string>(e, () => null);

                EnumerableAssert.AreEqual(e, export.Metadata);
            }
        }

        [TestMethod]
        public void Constructor3_NullAsMetadataArgument_ShouldSetDefinitionMetadataPropertyToEmptyDictionary()
        {
            var export = new Export<string, string>((IDictionary<string, object>)null, () => null); ;

            EnumerableAssert.IsEmpty(export.Definition.Metadata);
        }

        [TestMethod]
        public void Constructor3_NullAsMetadataArgument_ShouldSetDefinitionMetadataPropertyToReadOnlyDictionary()
        {
            var export = new Export<string, string>((IDictionary<string, object>)null, () => null);

            ExceptionAssert.Throws<NotSupportedException>(() =>
            {
                export.Definition.Metadata["Value"] = "Value";
            });
        }

        [TestMethod]
        public void Constructor3_WritableDictionaryAsMetadataArgument_ShouldSetDefinitionMetadataPropertyToReadOnlyDictionary()
        {
            var export = new Export<string, string>(new Dictionary<string, object>(), () => null);

            ExceptionAssert.Throws<NotSupportedException>(() =>
            {
                export.Definition.Metadata["Value"] = "Value";
            });
        }

        [TestMethod]
        public void Constructor3_DictionaryAsMetadataArgument_ShouldSetDefinitionMetadataProperty()
        {
            var expectations = Expectations.GetMetadata();

            foreach (var e in expectations)
            {
                var export = new Export<string, string>(e, () => null);

                EnumerableAssert.AreEqual(e, export.Definition.Metadata);
            }
        }

        [TestMethod]
        public void Constructor4_NullAsMetadataArgument_ShouldSetMetadataPropertyToEmptyDictionary()
        {
            var export = new Export<string, string>("ContractName", (IDictionary<string, object>)null, () => null); ;

            EnumerableAssert.IsEmpty(export.Metadata);
        }

        [TestMethod]
        public void Constructor4_NullAsMetadataArgument_ShouldSetMetadataPropertyToReadOnlyDictionary()
        {
            var export = new Export<string, string>("ContractName", (IDictionary<string, object>)null, () => null);

            ExceptionAssert.Throws<NotSupportedException>(() =>
            {
                export.Metadata["Value"] = "Value";
            });
        }

        [TestMethod]
        public void Constructor4_WritableDictionaryAsMetadataArgument_ShouldSetMetadataPropertyToReadOnlyDictionary()
        {
            var export = new Export<string, string>("ContractName", new Dictionary<string, object>(), () => null);

            ExceptionAssert.Throws<NotSupportedException>(() =>
            {
                export.Metadata["Value"] = "Value";
            });
        }

        [TestMethod]
        public void Constructor4_DictionaryAsMetadataArgument_ShouldSetMetadataProperty()
        {
           var expectations = Expectations.GetMetadata();

           foreach (var e in expectations)
           {
               var export = new Export<string, string>("ContractName", e, () => null);

               EnumerableAssert.AreEqual(e, export.Metadata);
           }
        }

        [TestMethod]
        public void Constructor4_NullAsMetadataArgument_ShouldSetDefinitionMetadataPropertyToEmptyDictionary()
        {
            var export = new Export<string, string>("ContractName", (IDictionary<string, object>)null, () => null); ;

            EnumerableAssert.IsEmpty(export.Definition.Metadata);
        }

        [TestMethod]
        public void Constructor4_NullAsMetadataArgument_ShouldSetDefinitionMetadataPropertyToReadOnlyDictionary()
        {
            var export = new Export<string, string>("ContractName", (IDictionary<string, object>)null, () => null);

            ExceptionAssert.Throws<NotSupportedException>(() =>
            {
                export.Definition.Metadata["Value"] = "Value";
            });
        }

        [TestMethod]
        public void Constructor4_WritableDictionaryAsMetadataArgument_ShouldSetDefinitionMetadataPropertyToReadOnlyDictionary()
        {
            var export = new Export<string, string>("ContractName", new Dictionary<string, object>(), () => null);

            ExceptionAssert.Throws<NotSupportedException>(() =>
            {
                export.Definition.Metadata["Value"] = "Value";
            });
        }

        [TestMethod]
        public void Constructor4_DictionaryAsMetadataArgument_ShouldSetDefinitionMetadataProperty()
        {
            var expectations = Expectations.GetMetadata();

            foreach (var e in expectations)
            {
                var export = new Export<string, string>("ContractName", e, () => null);

                EnumerableAssert.AreEqual(e, export.Definition.Metadata);
            }
        }

        [TestMethod]
        public void Constructor2_FuncReturningAStringAsExportedObjectGetter_ShouldBeReturnedByGetExportedObject()
        {
            var definition = ExportDefinitionFactory.Create();

            var export = new Export<string, string>(definition, () => "Value");

            Assert.AreEqual("Value", export.GetExportedObject());
        }

        [TestMethod]
        public void Constructor3_FuncReturningAStringAsExportedObjectGetter_ShouldBeReturnedByGetExportedObject()
        {
            var metadata = new Dictionary<string, object>();

            var export = new Export<string, string>(metadata, () => "Value");

            Assert.AreEqual("Value", export.GetExportedObject());
        }

        [TestMethod]
        public void Constructor4_FuncReturningAStringAsExportedObjectGetter_ShouldBeReturnedByGetExportedObject()
        {
            var export = new Export<string, string>("ContractName", new Dictionary<string, object>(), () => "Value");

            Assert.AreEqual("Value", export.GetExportedObject());
        }

        [TestMethod]
        public void Constructor2_FuncReturningNullAsExportedObjectGetter_ShouldBeReturnedByGetExportedObject()
        {
            var definition = ExportDefinitionFactory.Create();

            var export = new Export<string, string>(definition, () => null);

            Assert.IsNull(export.GetExportedObject());
        }

        [TestMethod]
        public void Constructor3_FuncReturningNullAsExportedObjectGetter_ShouldBeReturnedByGetExportedObject()
        {
            var metadata = new Dictionary<string, object>();

            var export = new Export<string, string>(metadata, () => null);

            Assert.IsNull(export.GetExportedObject());
        }

        [TestMethod]
        public void Constructor4_FuncReturningNullAsExportedObjectGetter_ShouldBeReturnedByGetExportedObject()
        {
            var export = new Export<string, string>("ContractName", new Dictionary<string, object>(), () => null);

            Assert.IsNull(export.GetExportedObject());
        }

        [TestMethod]
        public void Metadata_ShouldReturnOverriddenDefinitionMetadata()
        {
            var metadata = new Dictionary<string, object>();
            metadata["Name"] = "Value";

            var definition = ExportDefinitionFactory.Create("ContractName", metadata);

            var export = new DerivedExport<string, string>(definition);

            EnumerableAssert.AreEqual(metadata, export.Metadata);
        }

        [TestMethod]
        public void Definition_WhenNotOverridden_ShouldThrowNotImplemented()
        {
            var export = new NoOverridesExport<string, string>();

            ExceptionAssert.Throws<NotImplementedException>(() =>
            {
                var definition = export.Definition;
            });
        }

        [TestMethod]
        public void Metadata_WhenDefinitionNotOverridden_ShouldThrowNotImplemented()
        {
            var export = new NoOverridesExport<string, string>();

            ExceptionAssert.Throws<NotImplementedException>(() =>
            {
                var definition = export.Metadata;
            });
        }

        [TestMethod]
        public void MetadataView_WhenDefinitionNotOverridden_ShouldThrowNotImplemented()
        {
            var export = new NoOverridesExport<string, string>();

            ExceptionAssert.Throws<NotImplementedException>(() =>
            {
                var definition = export.MetadataView;
            });
        }

        [TestMethod]
        public void GetExportedObject_WhenGetExportedObjectCoreNotOverridden_ShouldThrowNotImplemented()
        {
            var export = new NoOverridesExport<string, string>();

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

            var export = new DerivedExport<int, string>(() =>
            {
                count++;
                return count;
            });

            Assert.AreEqual(1, export.GetExportedObject());
            Assert.AreEqual(2, export.GetExportedObject());
            Assert.AreEqual(3, export.GetExportedObject());
        }

        [TestMethod]
        public void MetadataView_IDictionaryAsMetadataArgument_ShouldReturnUnderlyingMetadata()
        {
            var metadata = new Dictionary<string, object>();
            metadata["Name"] = "Value";

            var export = new Export<string, IDictionary<string, object>>("ContractName", metadata, () => "Value");

            EnumerableAssert.AreEqual(metadata, export.MetadataView);
        }

        [TestMethod]
        public void MetadataView_IMetadataViewAsTMetadataViewArgument()
        {
            var metadata = new Dictionary<string, object>();
            metadata["Foo"] = "Value";

            var export = new Export<string, IMetadataView>("ContractName", metadata, () => "Value");

            Assert.AreEqual("Value", export.MetadataView.Foo);
            Assert.IsNull(export.MetadataView.OptionalFoo);
        }

        [TestMethod]
        public void MetadataView_ShouldCacheMetadataView()
        {
            var metadata = new Dictionary<string, object>();
            metadata["Foo"] = "Value";

            var export = new DerivedExport<string, IMetadataView>(metadata);

            Assert.IsNotNull(export.MetadataView);
            Assert.AreEqual(1, export.DefinitionAccessCount);

            Assert.IsNotNull(export.MetadataView);
            Assert.AreEqual(1, export.DefinitionAccessCount);
        }

        [TestMethod]
        public void MetadataView_StringAsTMetadataViewArgument_ShouldThrowMissingMethodException()
        {
            var export = new Export<string, string>("ContractName", new Dictionary<string, object>(), () => "Value");

            ExceptionAssert.Throws<MissingMethodException>(RetryMode.Retry, () =>
            {
                var metadataView = export.MetadataView;
            });
        }

        private class NoOverridesExport<T, TMetadataView> : Export<T, TMetadataView>
        {
        }

        private class DerivedExport<T, TMetadataView> : Export<T, TMetadataView>
        {
            private readonly Func<T> _exportedObjectGetter;
            private readonly ExportDefinition _definition;
            private int _definitionAccessCount;

            public DerivedExport(ExportDefinition definition)
            {
                _definition = definition;
            }

            public DerivedExport(Func<T> exportedObjectGetter)
            {
                _exportedObjectGetter = exportedObjectGetter;
            }

            public DerivedExport(IDictionary<string, object> metadata)
                : this(ExportDefinitionFactory.Create("ContractName", metadata))
            {
            }

            public int DefinitionAccessCount
            {
                get { return _definitionAccessCount; }
            }

            public override ExportDefinition Definition
            {
                get
                {
                    _definitionAccessCount++;
                    return _definition;
                }
            }

            protected override object GetExportedObjectCore()
            {
                return _exportedObjectGetter();
            }
        }
    }
}