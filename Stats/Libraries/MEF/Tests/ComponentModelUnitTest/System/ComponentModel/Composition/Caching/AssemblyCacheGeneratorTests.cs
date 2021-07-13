// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection.Emit;
using System.Reflection;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Caching;
using System.Collections;
using System.UnitTesting;
using System.ComponentModel.Composition.AttributedModel;
using System.ComponentModel.Composition.Factories;
using System.ComponentModel.Composition.Primitives;

namespace System.ComponentModel.Composition.Caching
{
    [TestClass]
    public class AssemblyCacheGeneratorTests
    {
        public class MyPart
        {
            
            [Import("FooImport")]
            public object FooImport { get; set; }

            [Export("FooExport")]
            [ExportMetadata("Foo", "Bar")]
            [ExportMetadata("Number", 42)]
            public object FooExport { get; set; }
        }

        internal static AssemblyCacheGenerator BeginGeneration(string catalogIdentifier)
        {
            string path = FileIO.GetTemporaryFileName("cache.dll");
            string assemblyNameString = Path.GetFileNameWithoutExtension(path);

            // build assembly/module
            AssemblyName assemblyName = new AssemblyName(assemblyNameString);
            AssemblyBuilder assemblyBuilder = System.Threading.Thread.GetDomain().DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Save, Path.GetDirectoryName(path));
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyNameString);

            AssemblyCacheGenerator generator = new AssemblyCacheGenerator(moduleBuilder, new ConcreteCachedComposablePartCatalogSite(), catalogIdentifier);
            generator.BeginGeneration();
            return generator;
        }

        internal static Type EndGeneration(AssemblyCacheGenerator generator)
        {
            generator.EndGeneration();
            AssemblyBuilder assemblyBuilder = generator.ModuleBuilder.Assembly as AssemblyBuilder;
            Assert.IsNotNull(assemblyBuilder);
            string assemblyFileName = new AssemblyName(assemblyBuilder.FullName).Name;
            assemblyBuilder.Save(assemblyFileName);

            AssemblyName assemblyName = new AssemblyName(assemblyBuilder.FullName);
            assemblyName.CodeBase = Path.Combine(FileIO.GetRootTemporaryDirectory(), assemblyFileName); 
            Assembly assembly = AppDomain.CurrentDomain.Load(assemblyName);
            Type stubType = assembly.GetType(CacheStructureConstants.CachingStubTypeNamePrefix + generator.CatalogIdentifier);
            Assert.IsNotNull(stubType);

            FieldInfo importDefinitionFactoryField = stubType.GetField(CacheStructureConstants.CachingStubImportDefinitionFactoryFieldName);
            Assert.IsNotNull(importDefinitionFactoryField);
            importDefinitionFactoryField.SetValue(null, new Func<ComposablePartDefinition, IDictionary<string, object>, ImportDefinition>(generator.CatalogSite.CreateImportDefinitionFromCache));

            FieldInfo exportDefinitionFactoryField = stubType.GetField(CacheStructureConstants.CachingStubExportDefinitionFactoryFieldName);
            Assert.IsNotNull(exportDefinitionFactoryField);
            exportDefinitionFactoryField.SetValue(null, new Func<ComposablePartDefinition, IDictionary<string, object>, ExportDefinition>(generator.CatalogSite.CreateExportDefinitionFromCache));

            FieldInfo partDefinitionFactoryField = stubType.GetField(CacheStructureConstants.CachingStubPartDefinitionFactoryFieldName);
            Assert.IsNotNull(partDefinitionFactoryField);
            partDefinitionFactoryField.SetValue(null, new Func<IDictionary<string, object>, Func<ComposablePartDefinition, IEnumerable<ImportDefinition>>, Func<ComposablePartDefinition, IEnumerable<ExportDefinition>>, ComposablePartDefinition>(generator.CatalogSite.CreatePartDefinitionFromCache));

            return stubType;
        }


        [TestMethod]
        public void VerifyCachingStub()
        {
            AssemblyCacheGenerator generator = BeginGeneration("Foo");
            Assembly assembly = EndGeneration(generator).Assembly;

            Type stub = assembly.GetType(CacheStructureConstants.CachingStubTypeNamePrefix + "Foo");
            Assert.IsNotNull(stub);
            Assert.IsTrue(stub.IsAbstract && stub.IsSealed && stub.IsPublic);

            FieldInfo exportFactoryField = stub.GetField(CacheStructureConstants.CachingStubExportDefinitionFactoryFieldName, BindingFlags.Static | BindingFlags.Public);
            Assert.IsNotNull(exportFactoryField);
            Assert.IsTrue(exportFactoryField.IsPublic);

            FieldInfo importFactoryField = stub.GetField(CacheStructureConstants.CachingStubImportDefinitionFactoryFieldName, BindingFlags.Static | BindingFlags.Public);
            Assert.IsNotNull(importFactoryField);
            Assert.IsTrue(importFactoryField.IsPublic);

            FieldInfo partFactoryField = stub.GetField(CacheStructureConstants.CachingStubPartDefinitionFactoryFieldName, BindingFlags.Static | BindingFlags.Public);
            Assert.IsNotNull(partFactoryField);
            Assert.IsTrue(partFactoryField.IsPublic);

            MethodInfo createExportMethod = stub.GetMethod(CacheStructureConstants.CachingStubCreateExportDefinitionMethodName, BindingFlags.Static | BindingFlags.NonPublic);
            Assert.IsNotNull(createExportMethod);
            MethodInfo createImportMethod = stub.GetMethod(CacheStructureConstants.CachingStubCreateImportDefinitionMethodName, BindingFlags.Static | BindingFlags.NonPublic);
            Assert.IsNotNull(createImportMethod);
            MethodInfo createPartMethod = stub.GetMethod(CacheStructureConstants.CachingStubCreatePartDefinitionMethodName, BindingFlags.Static | BindingFlags.NonPublic);
            Assert.IsNotNull(createPartMethod);
            MethodInfo contractMappingMethod = stub.GetMethod(CacheStructureConstants.CachingStubCreateContractNameToPartDefinitionMappingMethodName, BindingFlags.Static | BindingFlags.Public);
            Assert.IsNotNull(contractMappingMethod);
            MethodInfo getMetadataMethod = stub.GetMethod(CacheStructureConstants.CachingStubGetCatalogMetadata, BindingFlags.Static | BindingFlags.Public);
            Assert.IsNotNull(getMetadataMethod);
            MethodInfo getIdentifierMethod = stub.GetMethod(CacheStructureConstants.CachingStubGetCatalogIdentifier, BindingFlags.Static | BindingFlags.Public);
            Assert.IsNotNull(getIdentifierMethod);

            Dictionary<string, object> expectedDictionary = new Dictionary<string, object>();
            ExportDefinition expectedExport = ExportDefinitionFactory.Create("Test", expectedDictionary);
            ImportDefinition expectedImport = ImportDefinitionFactory.Create(export => true, ImportCardinality.ExactlyOne, true, true);
            Func<ComposablePartDefinition, IEnumerable<ImportDefinition>> expectedImportsFactory = delegate { return null; };
            Func<ComposablePartDefinition, IEnumerable<ExportDefinition>> expectedExportsFactory = delegate { return null; };
            ComposablePartDefinition expectedPart = PartDefinitionFactory.Create(expectedDictionary, () => null, () => null, () => null);

            Func<ComposablePartDefinition, IDictionary<string, object>, ExportDefinition> exportDefinitionFactory = delegate(ComposablePartDefinition  owner, IDictionary<string, object> dictionary)
            {
                Assert.AreSame(expectedPart, owner);
                Assert.AreSame(expectedDictionary, dictionary);
                return expectedExport;
            };


            Func<ComposablePartDefinition, IDictionary<string, object>, ImportDefinition> importDefinitionFactory = delegate(ComposablePartDefinition owner, IDictionary<string, object> dictionary)
            {
                Assert.AreSame(expectedPart, owner);
                Assert.AreSame(expectedDictionary, dictionary);
                return expectedImport;
            };

            Func<IDictionary<string, object>, Func<ComposablePartDefinition, IEnumerable<ImportDefinition>>, Func<ComposablePartDefinition, IEnumerable<ExportDefinition>>, ComposablePartDefinition> partDefinitionFactory = delegate(IDictionary<string, object> dictionary, Func<ComposablePartDefinition, IEnumerable<ImportDefinition>> importsFactory, Func<ComposablePartDefinition, IEnumerable<ExportDefinition>> exportsFactory)
            {
                Assert.AreSame(expectedDictionary, dictionary);
                Assert.AreSame(expectedImportsFactory, importsFactory);
                Assert.AreSame(expectedExportsFactory, exportsFactory);
                return expectedPart;
            };


            exportFactoryField.SetValue(null, exportDefinitionFactory);
            importFactoryField.SetValue(null, importDefinitionFactory);
            partFactoryField.SetValue(null, partDefinitionFactory);

            Assert.AreSame(expectedExport, createExportMethod.Invoke(null, new object[] { expectedPart, expectedDictionary }));
            Assert.AreSame(expectedImport, createImportMethod.Invoke(null, new object[] { expectedPart, expectedDictionary }));
            Assert.AreSame(expectedPart, createPartMethod.Invoke(null, new object[] { expectedDictionary, expectedImportsFactory, expectedExportsFactory }));
        }

        [TestMethod]
        public void VerifyDefinitionsTables()
        {
            AssemblyCacheGenerator generator = BeginGeneration("Foo");
            Assembly assembly = EndGeneration(generator).Assembly;

            Type partDefinitions = assembly.GetType(CacheStructureConstants.PartDefinitionTableNamePrefix + "Foo");
            Assert.IsNotNull(partDefinitions);
            Assert.IsTrue(partDefinitions.IsAbstract && partDefinitions.IsSealed && partDefinitions.IsPublic);


            Type exportsDefinitions = assembly.GetType(CacheStructureConstants.ExportsDefinitionTableNamePrefix + "Foo");
            Assert.IsNotNull(exportsDefinitions);
            Assert.IsTrue(exportsDefinitions.IsAbstract && exportsDefinitions.IsSealed && exportsDefinitions.IsNotPublic);


            Type importsDefinitions = assembly.GetType(CacheStructureConstants.ImportsDefinitionTableNamePrefix + "Foo");
            Assert.IsNotNull(importsDefinitions);
            Assert.IsTrue(importsDefinitions.IsAbstract && importsDefinitions.IsSealed && importsDefinitions.IsNotPublic);
        }

        [TestMethod]
        public void VerifyCacheSimplePart()
        {
            AssemblyCacheGenerator generator = BeginGeneration("Foo");
            ComposablePartDefinition expectedPartDefinition = PartDefinitionFactory.CreateAttributed(typeof(MyPart));
            CompositionResult<MethodInfo> generationResult = generator.CachePartDefinition(expectedPartDefinition);
            Assembly assembly = EndGeneration(generator).Assembly;

            Assert.IsTrue(generationResult.Succeeded);
            Type partsFactoryType = assembly.GetType(generationResult.Value.DeclaringType.FullName);
            Assert.IsNotNull(partsFactoryType);
            MethodInfo partFactory = partsFactoryType.GetMethod(generationResult.Value.Name, BindingFlags.Static | BindingFlags.Public);
            Assert.IsNotNull(partFactory);

            Func<ComposablePartDefinition> partDefinitionDelegate = Delegate.CreateDelegate(typeof(Func<ComposablePartDefinition>), partFactory) as Func<ComposablePartDefinition>;
            Assert.IsNotNull(partDefinitionDelegate);
            ComposablePartDefinition partDefinition = partDefinitionDelegate();
            Assert.IsNotNull(partDefinition);

            EnumerableAssert.AreEqual(expectedPartDefinition.Metadata, partDefinition.Metadata);

            Assert.AreEqual(expectedPartDefinition.ImportDefinitions.Count(), partDefinition.ImportDefinitions.Count());
            ContractBasedImportDefinition expectedImportDefinition = expectedPartDefinition.ImportDefinitions.Cast<ContractBasedImportDefinition>().First();
            ContractBasedImportDefinition importDefinition = partDefinition.ImportDefinitions.Cast<ContractBasedImportDefinition>().First();
            Assert.AreEqual(expectedImportDefinition.ContractName, importDefinition.ContractName);
            Assert.AreEqual(expectedImportDefinition.Cardinality, importDefinition.Cardinality);
            Assert.AreEqual(expectedImportDefinition.IsPrerequisite, importDefinition.IsPrerequisite);
            Assert.AreEqual(expectedImportDefinition.IsRecomposable, importDefinition.IsRecomposable);


            Assert.AreEqual(expectedPartDefinition.ExportDefinitions.Count(), partDefinition.ExportDefinitions.Count());
            ExportDefinition expectedExport = expectedPartDefinition.ExportDefinitions.First();
            ExportDefinition exportDefinition = partDefinition.ExportDefinitions.First();
            Assert.AreEqual(expectedExport.ContractName, exportDefinition.ContractName);
            EnumerableAssert.AreEqual(expectedExport.Metadata, exportDefinition.Metadata);

        }

        [TestMethod]
        [Ignore]
        [WorkItem(507696)]
        public void VerifyCacheMetadata()
        {
            IDictionary<string, object> expectedMetadata = new Dictionary<string, object>();
            expectedMetadata.Add("A", 42);
            expectedMetadata.Add("B", "Foo");
            expectedMetadata.Add("C", BindingFlags.ExactBinding);
            expectedMetadata.Add("D", this.GetType());

            AssemblyCacheGenerator generator = BeginGeneration("Foo");
            generator.CacheCatalogMetadata(expectedMetadata);
            Type stubType = EndGeneration(generator);

            MethodInfo getMetadataMethod = stubType.GetMethod(CacheStructureConstants.CachingStubGetCatalogMetadata, BindingFlags.Public | BindingFlags.Static);
            Assert.IsNotNull(getMetadataMethod);
            Func<IDictionary<string, object>> getMetadata = (Func<IDictionary<string, object>>)Delegate.CreateDelegate(typeof(Func<IDictionary<string, object>>), getMetadataMethod);
            Assert.IsNotNull(getMetadataMethod);
            IDictionary<string, object> metadata = getMetadata.Invoke();
            EnumerableAssert.AreEqual(expectedMetadata, metadata);
        }

        [TestMethod]
        public void VerifyCacheIdentifier()
        {
            AssemblyCacheGenerator generator = BeginGeneration("Foo");
            Type stubType = EndGeneration(generator);

            MethodInfo getIdentifierMethod = stubType.GetMethod(CacheStructureConstants.CachingStubGetCatalogIdentifier, BindingFlags.Public | BindingFlags.Static);
            Assert.IsNotNull(getIdentifierMethod);
            Func<string> getIdentifier = (Func<string>)Delegate.CreateDelegate(typeof(Func<string>), getIdentifierMethod);
            Assert.IsNotNull(getIdentifierMethod);
            string identifier = getIdentifier.Invoke();
            Assert.AreEqual("Foo", identifier);
        }

        [TestMethod]
        public void VerifyContractMapping()
        {
            AssemblyCacheGenerator generator = BeginGeneration("Foo");
            ComposablePartDefinition expectedPartDefinition = PartDefinitionFactory.CreateAttributed(typeof(MyPart));
            CompositionResult<MethodInfo> generationResult = generator.CachePartDefinition(expectedPartDefinition);
            Assert.IsTrue(generationResult.Succeeded);
            Assembly assembly = EndGeneration(generator).Assembly;

            Assert.IsTrue(generationResult.Succeeded);
            Type partsFactoryType = assembly.GetType(generationResult.Value.DeclaringType.FullName);
            Assert.IsNotNull(partsFactoryType);
            MethodInfo expectedPartFactoryMethod = partsFactoryType.GetMethod(generationResult.Value.Name, BindingFlags.Static | BindingFlags.Public);
            Assert.IsNotNull(expectedPartFactoryMethod);
            Func<ComposablePartDefinition> expectedPartFactory = Delegate.CreateDelegate(typeof(Func<ComposablePartDefinition>), expectedPartFactoryMethod) as Func<ComposablePartDefinition>;
            Assert.IsNotNull(expectedPartFactory);


            Type stub = assembly.GetType(CacheStructureConstants.CachingStubTypeNamePrefix + "Foo");
            Assert.IsNotNull(stub);
            MethodInfo contractMappingMethod = stub.GetMethod(CacheStructureConstants.CachingStubCreateContractNameToPartDefinitionMappingMethodName, BindingFlags.Static | BindingFlags.Public);
            Assert.IsNotNull(contractMappingMethod);
            Func<IDictionary<string, IEnumerable<Func<ComposablePartDefinition>>>> contractMappingDelegate = (Func<IDictionary<string, IEnumerable<Func<ComposablePartDefinition>>>>)Delegate.CreateDelegate(typeof(Func<IDictionary<string, IEnumerable<Func<ComposablePartDefinition>>>>), contractMappingMethod);
            IDictionary<string, IEnumerable<Func<ComposablePartDefinition>>> contractMapping = contractMappingDelegate.Invoke();

            Assert.IsNotNull(contractMapping);
            Assert.IsTrue(contractMapping.ContainsKey("FooExport"));
            IEnumerable<Func<ComposablePartDefinition>> fooExportParts = contractMapping["FooExport"];
            Assert.IsNotNull(fooExportParts);
            Assert.IsTrue(fooExportParts.Count() == 1);
            Func<ComposablePartDefinition> partFactory = fooExportParts.First();
            Assert.AreEqual(expectedPartFactory, partFactory);
        }
    }
}
