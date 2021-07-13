// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Factories;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Internal.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.UnitTesting;

namespace System.ComponentModel.Composition
{
    [TestClass]
    public class ComposablePartCatalogExportProviderTests
    {
        [TestMethod]
        public void Constructor_NullAsCatalogArgument_ShouldThrowArgumentNull()
        {
            ExceptionAssert.ThrowsArgument<ArgumentNullException>("catalog", () =>
            {
                new CatalogExportProvider((ComposablePartCatalog)null);
            });
        }

        [TestMethod]
        public void Constructor_ValueAsCatalogArgument_ShouldSetCatalogPropertyToEmpty()
        {
            var expectations = Expectations.GetCatalogs();

            foreach (var e in expectations)
            {
                var provider = new CatalogExportProvider(e);

                Assert.AreSame(e, provider.Catalog);
            }
        }

        [TestMethod]
        public void Catalog_WhenDisposed_ShouldThrowObjectDisposed()
        {
            var provider = CreateExportProvider();
            provider.Dispose();

            ExceptionAssert.ThrowsDisposed(provider, () =>
            {
                var catalog = provider.Catalog;
            });
        }

        [TestMethod]
        public void SourceProvider_NullAsValueArgument_ShouldThrowArgumentNull()
        {
            var provider = CreateExportProvider();

            ExceptionAssert.ThrowsArgument<ArgumentNullException>("value", () =>
            {
                provider.SourceProvider = null;
            });
        }

        private static CatalogExportProvider CreateExportProvider()
        {
            return new CatalogExportProvider(CatalogFactory.Create());
        }

        [TestMethod]
        [TestProperty("Type", "Integration")]
        public void BasicTest()
        {
            var catalog = CatalogFactory.CreateDefaultAttributed();
            var catalogExportProvider = new CatalogExportProvider(catalog);
            catalogExportProvider.SourceProvider = catalogExportProvider;
            var testName = AttributedModelServices.GetContractName(typeof(CatalogComponentTest));
            var testNameNonComponent = AttributedModelServices.GetContractName(typeof(CatalogComponentTestNonComponentPart));
            var testInterfaceName = AttributedModelServices.GetContractName(typeof(ICatalogComponentTest));

            Assert.AreEqual(1, catalogExportProvider.GetExports(ImportFromContract(testName)).Count());
            Assert.AreEqual(0, catalogExportProvider.GetExports(ImportFromContract(testNameNonComponent)).Count());

            var exports = catalogExportProvider.GetExports(ImportFromContract(testInterfaceName));
            Assert.AreEqual(2, exports.Count(), "There should be 2 of them");

            foreach (var i in exports)
                Assert.IsNotNull(i.GetExportedObject(), "Should get a value");

        }

        [TestMethod]
        [TestProperty("Type", "Integration")]
        public void BasicTestWithRequiredMetadata()
        {
            var catalog = CatalogFactory.CreateDefaultAttributed();
            var catalogExportProvider = new CatalogExportProvider(catalog);
            catalogExportProvider.SourceProvider = catalogExportProvider;

            Assert.AreEqual(0, catalogExportProvider.GetExports(ImportFromContractAndMetadata("MyExporterWithNoFoo", new string[] { "Foo" })).Count());
            Assert.AreEqual(1, catalogExportProvider.GetExports(ImportFromContractAndMetadata("MyExporterWithFoo", new string[] { "Foo" })).Count());
            Assert.AreEqual(0, catalogExportProvider.GetExports(ImportFromContractAndMetadata("MyExporterWithFoo", new string[] { "Foo", "Bar" })).Count());

            Assert.AreEqual(0, catalogExportProvider.GetExports(ImportFromContractAndMetadata("MyExporterWithNoFoo", new string[] { "Foo" })).Count());
            Assert.AreEqual(1, catalogExportProvider.GetExports(ImportFromContractAndMetadata("MyExporterWithFoo", new string[] { "Foo" })).Count());
        }

        [TestMethod]
        [TestProperty("Type", "Integration")]
        public void ComponentCatalogResolverGetStaticExport()
        {
            var catalog = CatalogFactory.CreateDefaultAttributed();
            var catalogExportProvider = new CatalogExportProvider(catalog);
            catalogExportProvider.SourceProvider = catalogExportProvider;

            var exports = catalogExportProvider.GetExports(ImportFromContract("StaticString"));
            Assert.AreEqual(1, exports.Count());
            Assert.AreEqual("StaticString", exports.First().GetExportedObject());
        }

        [TestMethod]
        [TestProperty("Type", "Integration")]
        public void ComponentCatalogResolverComponentCatalogExportReference()
        {
            var catalog = CatalogFactory.CreateDefaultAttributed();
            var catalogExportProvider = new CatalogExportProvider(catalog);
            catalogExportProvider.SourceProvider = catalogExportProvider;

            var exports = catalogExportProvider.GetExports(ImportFromContract(AttributedModelServices.GetContractName(typeof(MyExporterWithValidMetadata))));

            Assert.AreEqual(1, exports.Count());

            var export = exports.First();
            Assert.AreEqual("world", export.Metadata["hello"]);

            Assert.IsInstanceOfType(export.GetExportedObject(), typeof(MyExporterWithValidMetadata));
        }

        [TestMethod]
        [TestProperty("Type", "Integration")]
        public void ValueTypeFromCatalog()
        {
            var catalog = CatalogFactory.CreateDefaultAttributed();
            var container = new CompositionContainer(catalog);
            int singletonResult = container.GetExportedObject<int>("{AssemblyCatalogResolver}SingletonValueType");
            Assert.AreEqual(17, singletonResult, "expecting value type resolved from catalog");
            int factoryResult = container.GetExportedObject<int>("{AssemblyCatalogResolver}FactoryValueType");
            Assert.AreEqual(18, factoryResult, "expecting value type resolved from catalog");
        }

        [Export]
        [PartCreationPolicy(CreationPolicy.Any)]
        public class CreationPolicyAny
        {

        }

        [TestMethod]
        public void CreationPolicyAny_MultipleCallsReturnSameInstance()
        {
            var catalog = CatalogFactory.CreateAttributed(typeof (CreationPolicyAny));
            var provider = new CatalogExportProvider(catalog);
            provider.SourceProvider = ContainerFactory.Create();

            var export = provider.GetExportedObject<CreationPolicyAny>();

            for (int i = 0; i < 5; i++) // 5 is arbitrarily chosen
            {
                var export1 = provider.GetExportedObject<CreationPolicyAny>();

                Assert.AreEqual(export, export1);
            }
        }

        [Export]
        [PartCreationPolicy(CreationPolicy.Shared)]
        public class CreationPolicyShared
        {

        }

        [TestMethod]
        public void CreationPolicyShared_MultipleCallsReturnSameInstance()
        {
            var catalog = CatalogFactory.CreateAttributed(typeof(CreationPolicyShared));
            var provider = new CatalogExportProvider(catalog);
            provider.SourceProvider = ContainerFactory.Create();

            var export = provider.GetExportedObject<CreationPolicyShared>();

            for (int i = 0; i < 5; i++) // 5 is arbitrarily chosen
            {
                var export1 = provider.GetExportedObject<CreationPolicyShared>();

                Assert.AreEqual(export, export1);
            }
        }

        [Export]
        [PartCreationPolicy(CreationPolicy.NonShared)]
        public class CreationPolicyNonShared
        {

        }

        [TestMethod]
        public void CreationPolicyNonShared_MultipleCallsReturnsDifferentInstances()
        {
            var catalog = CatalogFactory.CreateAttributed(typeof(CreationPolicyNonShared));
            var provider = new CatalogExportProvider(catalog);
            provider.SourceProvider = ContainerFactory.Create();

            List<CreationPolicyNonShared> list = new List<CreationPolicyNonShared>();
            var export = provider.GetExportedObject<CreationPolicyNonShared>();
            list.Add(export);

            for (int i = 0; i < 5; i++) // 5 is arbitrarily chosen
            {
                export = provider.GetExportedObject<CreationPolicyNonShared>();

                CollectionAssert.DoesNotContain(list, export);
                list.Add(export);
            }
        }

        private static ImportDefinition ImportFromContract(string contractName)
        {
            return ImportDefinitionFactory.CreateDefault(contractName,
                                                     ImportCardinality.ZeroOrMore,
                                                     false,
                                                     false);
        }

        private static ImportDefinition ImportFromContractAndMetadata(string contractName, params string[] metadata)
        {
            return new ContractBasedImportDefinition(contractName,
                                                     (string)null,
                                                     metadata,
                                                     ImportCardinality.ZeroOrMore,
                                                     false,
                                                     false,
                                                     CreationPolicy.Any);
        }
    }
}
