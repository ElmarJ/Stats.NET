// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition.Factories;
using System.ComponentModel.Composition.UnitTesting;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.UnitTesting;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;

namespace System.ComponentModel.Composition
{
    // TODO: Rename and change these tests so that they are AdaptingExportProvider tests
    [TestClass]
    public class CompositionContainerAdapterTests
    {
        [TestMethod]
        public void ChainedAdapter_ShouldNotBeCalledToAdapt()
        {   // Tests that chaining adapters is not supported, ie that 
            // OldContract -> NewContract -> NewerContract does not work

            var adapter1 = AdapterFactory.CreateAdapter("OldContract", "NewContract");
            var adapter2 = AdapterFactory.CreateAdapter("NewContract", "NewerContract");

            var container = ContainerFactory.Create(new MicroExport("OldContract", 1, 2, 3));
            CompositionBatch batch = new CompositionBatch();
            batch.AddPart(adapter1);
            batch.AddPart(adapter2);
            container.Compose(batch);

            var exports = container.GetExports(typeof(object), (Type)null, "NewerContract");

            Assert.AreEqual(0, exports.Count);

            exports = container.GetExports(typeof(object), (Type)null, "NewContract");

            Assert.AreEqual(3, exports.Count);
        }

        [TestMethod]
        public void MultipleAdaptersForSameContract_ShouldDuplicateExports()
        {
            var adapter1 = AdapterFactory.CreateAdapter("OldContract", "NewContract");
            var adapter2 = AdapterFactory.CreateAdapter("OldContract", "NewContract");

            var container = ContainerFactory.Create(new MicroExport("OldContract", 1, 2, 3));
            CompositionBatch batch = new CompositionBatch();
            batch.AddPart(adapter1);
            batch.AddPart(adapter2);
            container.Compose(batch);

            var exports = container.GetExports(typeof(int), (Type)null, "NewContract");

            ExportsAssert.AreEqual(exports, 1, 2, 3, 1, 2, 3);
        }

        [TestMethod]
        public void AddingAdapterAfterCompose_ShouldRecomposeNewContractsViaGetExports()
        {
            var container = ContainerFactory.Create(new MicroExport("OldContract", 4, 5, 6),
                                                                     new MicroExport("NewContract", 1, 2, 3));

            var exports = container.GetExports<int>("NewContract");
            Assert.AreEqual(3, exports.Count);

            CompositionBatch batch = new CompositionBatch();
            batch.AddPart(AdapterFactory.CreateAdapter("OldContract", "NewContract"));
            container.Compose(batch);

            exports = container.GetExports<int>("NewContract");

            ExportsAssert.AreEqual(exports, 1, 2, 3, 4, 5, 6);
        }

        [TestMethod]
        public void AddingAdapterAfterCompose_ShouldRecomposeNewContractsViaParts()
        {
            var container = ContainerFactory.Create(new MicroExport("OldContract", "Value"));
            var importer = PartFactory.CreateImporter("NewContract", true);
            CompositionBatch batch = new CompositionBatch();
            batch.AddPart(importer);
            container.Compose(batch);
            Assert.IsNull(importer.Value);
            Assert.AreEqual(1, importer.ImportSatisfiedCount);

            batch = new CompositionBatch();
            batch.AddPart(AdapterFactory.CreateAdapter("OldContract", "NewContract"));            
            container.Compose(batch);

            Assert.AreEqual("Value", importer.Value);
            Assert.AreEqual(2, importer.ImportSatisfiedCount);
        }

        [TestMethod]
        public void RemovingAdapterAfterCompose_ShouldRecomposeNewContractsViaParts()
        {
            var container = ContainerFactory.Create(new MicroExport("OldContract", "Value"));
            CompositionBatch batch = new CompositionBatch();
            var importer = PartFactory.CreateImporter("NewContract", true);
            var adapter = AdapterFactory.CreateAdapter("OldContract", "NewContract");
            batch.AddPart(importer);
            batch.AddPart(adapter);
            container.Compose(batch);

            Assert.AreEqual("Value", importer.Value);
            Assert.AreEqual(1, importer.ImportSatisfiedCount);

            batch = new CompositionBatch();
            batch.RemovePart(adapter);
            container.Compose(batch);

            Assert.IsNull(importer.Value);
            Assert.AreEqual(2, importer.ImportSatisfiedCount);
        }

        [TestMethod]
        public void RemovingAndAddingAdapterAfterCompose_ShouldRecomposeNewContractsViaParts()
        {
            var container = ContainerFactory.Create(new MicroExport("OldContract", "Value"));
            CompositionBatch batch = new CompositionBatch();
            var importer = PartFactory.CreateImporter("NewContract", true);
            var adapter1 = AdapterFactory.CreateAdapter("OldContract", "NewContract");
            var adapter2 = AdapterFactory.CreateAdapter("OldContract", "NewContract", export => ExportFactory.Create("NewContract", () => "AnotherValue"));
            batch.AddPart(importer);
            batch.AddPart(adapter1);
            container.Compose(batch);

            Assert.AreEqual("Value", importer.Value);
            Assert.AreEqual(1, importer.ImportSatisfiedCount);

            batch = new CompositionBatch();
            batch.RemovePart(adapter1);
            batch.AddPart(adapter2);
            container.Compose(batch);

            Assert.AreEqual(2, importer.ImportSatisfiedCount);
            Assert.AreEqual("AnotherValue", importer.Value);
        }

        [TestMethod]
        public void AddingAdapter_ShouldNotRecomposeUnrelatedParts()
        {
            var adapter1 = AdapterFactory.CreateAdapter("OldContract1", "NewContract1");
            var importer = PartFactory.CreateImporter("NewContract1");
            var container = ContainerFactory.Create(adapter1, importer);
            CompositionBatch batch = new CompositionBatch();
            container.Compose(batch);

            Assert.AreEqual(1, importer.ImportSatisfiedCount);

            var adapter2 = AdapterFactory.CreateAdapter("OldContract2", "NewContract2");

            batch = new CompositionBatch();
            batch.AddPart(adapter2);
            container.Compose(batch);

            Assert.AreEqual(1, importer.ImportSatisfiedCount);
        }

        [TestMethod]
        public void RemovingAdapter_ShouldNotRecomposeUnrelatedParts()
        {
            var adapter1 = AdapterFactory.CreateAdapter("OldContract1", "NewContract1");
            var adapter2 = AdapterFactory.CreateAdapter("OldContract2", "NewContract2");
            var importer = PartFactory.CreateImporter("NewContract2");
            var container = ContainerFactory.Create(adapter1, adapter2, importer);
            CompositionBatch batch = new CompositionBatch();
            container.Compose(batch);

            Assert.AreEqual(1, importer.ImportSatisfiedCount);

            batch = new CompositionBatch();
            batch.RemovePart(adapter1);
            container.Compose(batch);

            Assert.AreEqual(1, importer.ImportSatisfiedCount);
        }

        [TestMethod]
        public void AddingAnAdaptedContract_ShouldRecomposeNewContractsViaGetExports()
        {
            var container = ContainerFactory.Create(new MicroExport("OldContract", 1, 2, 3));
            var adapter = AdapterFactory.CreateAdapter("OldContract", "NewContract");
            CompositionBatch batch = new CompositionBatch();
            batch.AddPart(adapter);
            container.Compose(batch);

            var exports = container.GetExports<int>("NewContract");
            Assert.AreEqual(3, exports.Count);

            batch = new CompositionBatch();
            batch.AddExportedObject("OldContract", 4);
            container.Compose(batch);

            exports = container.GetExports<int>("NewContract");
            ExportsAssert.AreEqual(exports, 1, 2, 3, 4);
        }

        [TestMethod]
        public void AddingAnAdaptedContract_ShouldRecomposeNewContractsViaParts()
        {
            var container = ContainerFactory.Create(); 
            CompositionBatch batch = new CompositionBatch();
            var adapter = AdapterFactory.CreateAdapter("OldContract", "NewContract");
            var importer = PartFactory.CreateImporter("NewContract", true);
            batch.AddPart(importer);
            batch.AddPart(adapter);
            container.Compose(batch);

            Assert.IsNull(importer.Value);
            Assert.AreEqual(1, importer.ImportSatisfiedCount);

            batch = new CompositionBatch();
            batch.AddExportedObject("OldContract", "Value");
            container.Compose(batch);

            Assert.AreEqual("Value", importer.Value);
            Assert.AreEqual(2, importer.ImportSatisfiedCount);
        }

        [TestMethod]
        public void RemovingAnAdapter_ShouldRecomposeNewContracts()
        {
            var adapter = AdapterFactory.CreateAdapter("OldContract", "NewContract");
            var container = ContainerFactory.Create(new MicroExport("OldContract", 1, 2, 3));
            CompositionBatch batch = new CompositionBatch();

            batch.AddPart(adapter);
            container.Compose(batch);

            var exports = container.GetExports(typeof(int), (Type)null, "NewContract");
            Assert.AreEqual(3, exports.Count);

            batch = new CompositionBatch();
            batch.RemovePart(adapter);
            container.Compose(batch);

            exports = container.GetExports(typeof(int), (Type)null, "NewContract");
            Assert.AreEqual(0, exports.Count);
        }

        [TestMethod]
        public void AdapterWithSameToAndFromContract_ShouldThrowComposition()
        {
            var adapter = AdapterFactory.CreateAdapter("Contract", "Contract");
            var element = (ICompositionElement)adapter.ExportDefinitions.First();
            var container = ContainerFactory.Create(new MicroExport("Contract", 1));
            CompositionBatch batch = new CompositionBatch();

            batch.AddPart(adapter);
            container.Compose(batch);

            CompositionAssert.ThrowsError(ErrorId.Adapter_CannotAdaptFromAndToSameContract, element, () =>
            {
                container.GetExports(typeof(int), (Type)null, "Contract");
            });
        }

        [TestMethod]
        public void AdapterWithSameToAndFromContractType_ShouldThrowComposition()
        {
            var adapter = AdapterFactory.CreateAdapter(typeof(string), typeof(string));
            var element = (ICompositionElement)adapter.ExportDefinitions.First();
            var container = ContainerFactory.Create(new MicroExport(typeof(string), 1));
            CompositionBatch batch = new CompositionBatch();
            batch.AddPart(adapter);
            container.Compose(batch);

            CompositionAssert.ThrowsError(ErrorId.Adapter_CannotAdaptFromAndToSameContract, element, () =>
            {
                container.GetExports(typeof(string), (Type)null, (string)null);
            });
        }

        [TestMethod]
        public void AdapterWithSameToAndFromContractWithNoExportsMatchingFromContract_ShouldThrowComposition()
        {
            var adapter = AdapterFactory.CreateAdapter("Contract", "Contract");
            var element = (ICompositionElement)adapter.ExportDefinitions.First();
            var container = ContainerFactory.Create(); 
            CompositionBatch batch = new CompositionBatch();
            batch.AddPart(adapter);
            container.Compose(batch);

            CompositionAssert.ThrowsError(ErrorId.Adapter_CannotAdaptFromAndToSameContract, element, () =>
            {
                container.GetExports(typeof(string), (Type)null, "Contract");
            });
        }

        [TestMethod]
        public void AdapterWithSameToAndFromContractTypeWithNoExportsMatchingFromContract_ShouldThrowComposition()
        {
            var adapter = AdapterFactory.CreateAdapter(typeof(string), typeof(string));
            var element = (ICompositionElement)adapter.ExportDefinitions.First();
            var container = ContainerFactory.Create(); 
            CompositionBatch batch = new CompositionBatch();
            batch.AddPart(adapter);
            container.Compose(batch);

            CompositionAssert.ThrowsError(ErrorId.Adapter_CannotAdaptFromAndToSameContract, element, () =>
            {
                container.GetExports(typeof(string), (Type)null, (string)null);
            });
        }

        [TestMethod]
        public void AdapterWithNullFromContract_ShouldThrowComposition()
        {
            var adapter = AdapterFactory.CreateAdapter((string)null, "Contract");
            var element = (ICompositionElement)adapter.ExportDefinitions.First();
            var container = ContainerFactory.Create(adapter);

            CompositionAssert.ThrowsError(ErrorId.Adapter_CannotAdaptNullOrEmptyFromOrToContract, element, () =>
            {
                container.GetExports(typeof(string), (Type)null, "Contract");
            });
        }

        [TestMethod]
        public void AdapterWithNullToContract_ShouldThrowComposition()
        {
            var adapter = AdapterFactory.CreateAdapter("OldContract", (string)null);
            var element = (ICompositionElement)adapter.ExportDefinitions.First();
            var container = ContainerFactory.Create(adapter);

            CompositionAssert.ThrowsError(ErrorId.Adapter_CannotAdaptNullOrEmptyFromOrToContract, element, () =>
            {
                container.GetExports(typeof(string), (Type)null, "Contract");
            });
        }

        [TestMethod]
        public void AdapterWithEmptyFromContract_ShouldThrowComposition()
        {
            var adapter = AdapterFactory.CreateAdapter("", "Contract");
            var element = (ICompositionElement)adapter.ExportDefinitions.First();
            var container = ContainerFactory.Create(adapter);

            var definition = ImportDefinitionFactory.Create("Contract");

            CompositionAssert.ThrowsError(ErrorId.Adapter_CannotAdaptNullOrEmptyFromOrToContract, element, () =>
            {
                container.GetExports(definition);
            });
        }

        [TestMethod]
        public void AdapterWithEmptyToContract_ShouldThrowComposition()
        {
            var adapter = AdapterFactory.CreateAdapter("OldContract", "");
            var element = (ICompositionElement)adapter.ExportDefinitions.First();
            var container = ContainerFactory.Create(adapter);

            var definition = ImportDefinitionFactory.Create("Contract");

            CompositionAssert.ThrowsError(ErrorId.Adapter_CannotAdaptNullOrEmptyFromOrToContract, element, () =>
            {
                container.GetExports(definition);
            });
        }

        [TestMethod]
        public void AdapterAlwaysReturningNull_ShouldNotAddToAvailableExports()
        {
            var adapter = AdapterFactory.CreateAdapter("OldContract", "NewContract", export => null);
            var container = ContainerFactory.Create(new MicroExport("OldContract", "Value"));
            CompositionBatch batch = new CompositionBatch();
            batch.AddPart(adapter);
            container.Compose(batch);

            var exports = container.GetExportedObjects<string>("NewContract");

            EnumerableAssert.IsEmpty(exports);
        }

        [TestMethod]
        public void AdapterSometimesReturningNull_ShouldNotAddToAvailableExports()
        {
            int count = 0;
            Func<Export, Export> adapt = export =>
            {
                count++;
                if (count % 2 == 0)
                {
                    return new Export(ExportDefinitionFactory.Create("NewContract", export.Metadata), () => export.GetExportedObject());
                }

                return null;
            };

            var adapter = AdapterFactory.CreateAdapter("OldContract", "NewContract", adapt);
            var container = ContainerFactory.Create(new MicroExport("OldContract", "Value1", "Value2"));
            CompositionBatch batch = new CompositionBatch();
            batch.AddPart(adapter);
            container.Compose(batch);

            var exports = container.GetExportedObjects<string>("NewContract");
            
            EnumerableAssert.AreEqual(exports, "Value2");
        }

        [TestMethod]
        public void AdapterAddedDuringCompositionWithImporterWithZeroOrMoreCardinality_ShouldBeUsed()
        {
            var adapter = AdapterFactory.CreateAdapter("OldContract", "NewContract");
            var importer = PartFactory.CreateImporter("NewContract", ImportCardinality.ZeroOrMore, true);

            var container = ContainerFactory.Create(new MicroExport("OldContract", "Value"));
            CompositionBatch batch = new CompositionBatch();
            batch.AddPart(importer);
            batch.AddPart(new CallbackExecuteCodeDuringCompose(() => 
                {
                    CompositionBatch nestedBatch = new CompositionBatch();
                    nestedBatch.AddPart(adapter);
                    container.Compose(nestedBatch);
                }));

            container.Compose(batch);

            Assert.AreEqual(2, importer.ImportSatisfiedCount);
            Assert.AreEqual(importer.Value, "Value");
        }

        [TestMethod]
        public void AdapterAddedDuringCompositionWithImporterWithZeroOrOneCardinality_ShouldBeUsed()
        {
            var adapter = AdapterFactory.CreateAdapter("OldContract", "NewContract");
            var importer = PartFactory.CreateImporter("NewContract", ImportCardinality.ZeroOrOne, true);

            var container = ContainerFactory.Create(new MicroExport("OldContract", "Value"));
            CompositionBatch batch = new CompositionBatch();
            batch.AddPart(importer);
            batch.AddPart(new CallbackExecuteCodeDuringCompose(() =>
            {
                CompositionBatch nestedBatch = new CompositionBatch();
                nestedBatch.AddPart(adapter);
                container.Compose(nestedBatch);
            }));

            container.Compose(batch);

            Assert.AreEqual(2, importer.ImportSatisfiedCount);
            Assert.AreEqual(importer.Value, "Value");
        }

        [TestMethod]
        public void ExportAddedDuringComposition_ShouldBeUsed()
        {
            var adapter = AdapterFactory.CreateAdapter("OldContract", "NewContract");
            var importer = PartFactory.CreateImporter("NewContract", ImportCardinality.ZeroOrMore, true);

            var container = ContainerFactory.Create(); 
            CompositionBatch batch = new CompositionBatch();
            batch.AddPart(importer);
            batch.AddPart(adapter);
            batch.AddPart(new CallbackExecuteCodeDuringCompose(() =>
            {
                CompositionBatch nestedBacth = new CompositionBatch();
                nestedBacth.AddExportedObject("OldContract", "Value");
                container.Compose(nestedBacth);
            }));

            container.Compose(batch);

            Assert.AreEqual(2, importer.ImportSatisfiedCount);
            Assert.AreEqual(importer.Value, "Value");
        }

        [TestMethod]
        public void PartWithAdaptMethodAndFromAndToMetadata_ShouldNotBeUsedAsAdapter()
        {
            var metadata = AdapterFactory.CreateAdapterMetadata("OldContract", "NewContract");
            Func<Export, Export> adapt = export => export;

            var container = ContainerFactory.Create(new MicroExport("NotAnAdapter", metadata, adapt),
                                                    new MicroExport("OldContract", "Value"));

            Assert.IsFalse(container.IsPresent("NewContract"));
        }

        [TestMethod]
        public void AdapterExportingNull_ShouldThrowComposition()
        {
            IDictionary<string, object> metadata = AdapterFactory.CreateAdapterMetadata("OldContract", "NewContract");

            var container = ContainerFactory.Create(new MicroExport("OldContract", 1),
                                                    new MicroExport(CompositionConstants.AdapterContractName, typeof(int), metadata, new object[] { null }));

            CompositionAssert.ThrowsError(ErrorId.Adapter_TypeMismatch, () =>
            {
                container.GetExports(typeof(int), (Type)null, "NewContract");
            });
        }

        [TestMethod]
        public void AdapterExportingInt32_ShouldThrowComposition()
        {
            IDictionary<string, object> metadata = AdapterFactory.CreateAdapterMetadata("OldContract", "NewContract");

            var container = ContainerFactory.Create(new MicroExport("OldContract", 1),
                                                    new MicroExport(CompositionConstants.AdapterContractName, metadata, 1));

            CompositionAssert.ThrowsError(ErrorId.Adapter_TypeMismatch, () =>
            {
                container.GetExports(typeof(int), (Type)null, "NewContract");
            });
        }

        [TestMethod]
        public void AdapterExportingFuncTakingExportOfT_ShouldThrowComposition()
        {
            IDictionary<string, object> metadata = AdapterFactory.CreateAdapterMetadata("OldContract", "NewContract");

            Func<Export<string>, Export> adapter = (e) => null;

            var container = ContainerFactory.Create(new MicroExport("OldContract", 1),
                                                    new MicroExport(CompositionConstants.AdapterContractName, metadata, adapter));

            CompositionAssert.ThrowsError(ErrorId.Adapter_TypeMismatch, () =>
            {
                container.GetExports(typeof(int), (Type)null, "NewContract");
            });
        }

        [TestMethod]
        public void AdapterExportingFuncReturningObject_ShouldThrowComposition()
        {
            IDictionary<string, object> metadata = AdapterFactory.CreateAdapterMetadata("OldContract", "NewContract");

            Func<Export, object> adapter = (e) => null;

            var container = ContainerFactory.Create(new MicroExport("OldContract", 1),
                                                    new MicroExport(CompositionConstants.AdapterContractName, metadata, adapter));

            CompositionAssert.ThrowsError(ErrorId.Adapter_TypeMismatch, () =>
            {
                container.GetExports(typeof(int), (Type)null, "NewContract");
            });
        }

        [TestMethod]
        public void AdapterThrowingDuringAdaptViaGetExport_ShouldThrowComposition()
        {
            Exception exceptionToThrow = new Exception();
            var adapter = AdapterFactory.CreateAdapter("OldContract", "NewContract", e =>
            {
                throw exceptionToThrow;
            });

            var adaptMethodDefinition = (ICompositionElement)adapter.ExportDefinitions.First();

            var container = ContainerFactory.Create(new MicroExport("OldContract", 1));
            CompositionBatch batch = new CompositionBatch();
            batch.AddPart(adapter);
            container.Compose(batch);

            CompositionAssert.ThrowsError(ErrorId.Adapter_ExceptionDuringAdapt, adaptMethodDefinition, exceptionToThrow, () =>
            {
                container.GetExports(typeof(int), (Type)null, "NewContract");
            });
        }

        [TestMethod]
        public void AdapterThrowingDuringAdaptViaPartWithOneImport_ShouldThrowComposition()
        {
            Exception exceptionToThrow = new Exception();
            var adapter = AdapterFactory.CreateAdapter("OldContract", "NewContract", export =>
            {
                throw exceptionToThrow;
            });

            var container = ContainerFactory.Create(new MicroExport("OldContract", "Value"));
            CompositionBatch batch = new CompositionBatch();
            batch.AddPart(adapter);
            container.Compose(batch);

            batch = new CompositionBatch();
            batch.AddPart(new ImportAdaptedContract());

            CompositionAssert.ThrowsError(ErrorId.CompositionEngine_PartCannotSetImport,
                                          ErrorId.Adapter_ExceptionDuringAdapt, RetryMode.DoNotRetry, () =>
            {
                container.Compose(batch);
            });
        }

        [TestMethod]
        public void AdapterThrowingDuringAdaptViaPartWithMultipleImports_ShouldThrowComposition()
        {
            Exception exceptionToThrow = new Exception();
            var adapter = AdapterFactory.CreateAdapter("OldContract", "NewContract", export =>
            {
                throw exceptionToThrow;
            });

            var container = ContainerFactory.Create(new MicroExport("OldContract", "Value"));
            CompositionBatch batch = new CompositionBatch();
            batch.AddPart(adapter);
            container.Compose(batch);

            batch = new CompositionBatch();
            batch.AddPart(new ImportAdaptedContracts());

            CompositionAssert.ThrowsError(ErrorId.CompositionEngine_PartCannotSetImport, 
                                          ErrorId.Adapter_ExceptionDuringAdapt, 
                                          RetryMode.DoNotRetry, () =>
            {
                container.Compose(batch);
            });
        }

        [TestMethod]
        public void AdapterReturningExportWithDifferentContractThanToContract_ShouldThrowComposition()
        {
            var adapter = AdapterFactory.CreateAdapter("OldContract", "NewContract", export => export);

            var container = ContainerFactory.Create(new MicroExport("OldContract", "Value"));
            CompositionBatch batch = new CompositionBatch();
            batch.AddPart(adapter);
            container.Compose(batch);

            batch = new CompositionBatch();
            batch.AddPart(new ImportAdaptedContract());

            CompositionAssert.ThrowsError(ErrorId.CompositionEngine_PartCannotSetImport,
                                          ErrorId.Adapter_ContractMismatch, 
                                          RetryMode.DoNotRetry, () =>
            {
                container.Compose(batch);
            });
        }

        [TestMethod]
        public void AdapterReturningExportWithDifferentContractThanToContractBasedOnCase_ShouldThrowComposition()
        {
            var adapter = AdapterFactory.CreateAdapter("OldContract", "NewContract", export => 
                {
                    return ExportFactory.Create("newcontract", () => export.GetExportedObject());                    
                });

            var container = ContainerFactory.Create(new MicroExport("OldContract", "Value"));
            CompositionBatch batch = new CompositionBatch();
            batch.AddPart(adapter);
            container.Compose(batch);

            batch = new CompositionBatch();
            batch.AddPart(new ImportAdaptedContract());

            CompositionAssert.ThrowsError(ErrorId.CompositionEngine_PartCannotSetImport, 
                                          ErrorId.Adapter_ContractMismatch, 
                                          RetryMode.DoNotRetry, () =>
            {
                container.Compose(batch);
            });
        }

        [TestMethod]
        public void RecomposingOfAnAdaptedContract_ShouldRecomposeNewContractOnce()
        {
            var importer = PartFactory.CreateImporter("NewContract", true);
            var adapter1 = AdapterFactory.CreateAdapter("OldContract", "NewContract");
            var adapter2 = AdapterFactory.CreateAdapter("OldContract", "NewContract");
            var container = ContainerFactory.Create(new MicroExport("OldContract", 1));
            CompositionBatch batch = new CompositionBatch();

            batch.AddParts(importer, adapter1, adapter2);
            container.Compose(batch);

            Assert.AreEqual(1, importer.ImportSatisfiedCount);

            batch = new CompositionBatch();
            batch.AddExportedObject("OldContract", 2);
            container.Compose(batch);

            Assert.AreEqual(2, importer.ImportSatisfiedCount);
        }

        [TestMethod]
        public void AdapterAloneInContainer_ShouldNotThrow()
        {
            var adapter = AdapterFactory.CreateAdapter("OldContract", "NewContract");
            var container = ContainerFactory.Create(adapter);
        }

        [TestMethod]
        public void AskingForToContractForAdapterWithNoFromContracts_ShouldReturnNull()
        {
            var adapter = AdapterFactory.CreateAdapter("OldContract", "NewContract");
            var container = ContainerFactory.Create(adapter);

            Assert.IsFalse(container.IsPresent("NewContract"));
        }

        [TestMethod]
        public void AdapterInContainer_DoesNotCauseExportToBePulledWhenAdapting()
        {
            var adapter = AdapterFactory.CreateAdapter("OldContract", "NewContract");
            var container = ContainerFactory.Create(adapter);
            CompositionBatch batch = new CompositionBatch();

            int calledCount = 0;

            batch.AddExport(ExportFactory.Create("OldContract", () =>
            {
                calledCount++;
                return "Value";
            }));

            container.Compose(batch);

            Assert.AreEqual(0, calledCount);

            var exports = container.GetExports(typeof(object), (Type)null, "NewContract");

            Assert.AreEqual(0, calledCount);
            Assert.AreEqual(1, exports.Count);
            Assert.AreEqual("Value", exports[0].GetExportedObject());
            Assert.AreEqual(1, calledCount);
        }

        [TestMethod]
        public void ReflectionAdapterInContainer_CanAdapt()
        {
            var container = ContainerFactory.Create(new MicroExport("OldContract", 1));
            CompositionBatch batch = new CompositionBatch();
            batch.AddPart(new Adapter());
            container.Compose(batch);

            var export = container.GetExport<int>("NewContract");

            Assert.AreEqual(1, export.GetExportedObject());
        }

        private static Expression<Func<ExportDefinition, bool>> ConstraintFromContract(string contractName)
        {
            return ConstraintFactory.Create(contractName);
        }

        public class Adapter
        {
            [Export(CompositionConstants.AdapterContractName)]
            [ExportMetadata(CompositionConstants.AdapterFromContractMetadataName, "OldContract")]
            [ExportMetadata(CompositionConstants.AdapterToContractMetadataName, "NewContract")]
            public Export Adapt(Export export)
            {
                return ExportFactory.Create("NewContract",
                                  export.Definition.Metadata,
                                  () => export.GetExportedObject());
            }
        }

        public class ImportAdaptedContract
        {
            [Import("NewContract")]
            public string Import
            {
                get;
                set;
            }
        }

        public class ImportAdaptedContracts
        {
            private Collection<string> _imports = new Collection<string>();

            [ImportMany("NewContract")]
            public Collection<string> Imports
            {
                get { return _imports; }
            }
        }
    }
}
