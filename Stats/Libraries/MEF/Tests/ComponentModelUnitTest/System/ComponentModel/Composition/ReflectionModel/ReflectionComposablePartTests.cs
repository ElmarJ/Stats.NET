// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.UnitTesting;
using System.ComponentModel.Composition.UnitTesting;
using System.ComponentModel.Composition.Factories;
using System.Collections.Generic;
using Microsoft.Internal;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.AttributedModel;

namespace System.ComponentModel.Composition.ReflectionModel
{
    [TestClass]
    public class ReflectionComposablePartTests
    {
        [TestMethod]
        public void Constructor1_DefinitionAsDefinitionArgument_ShouldSetOriginProperty()
        {
            var expectations = Expectations.GetAttributedDefinitions();

            foreach (var e in expectations)
            {
                var definition = (ICompositionElement)new ReflectionComposablePart(e);

                Assert.AreSame(e, definition.Origin);
            }
        }

        [TestMethod]
        public void Constructor1_NullAsDefinitionArgument_ShouldThrowArgumentNull()
        {
            ExceptionAssert.ThrowsArgument<ArgumentNullException>("definition", () =>
            {
                new ReflectionComposablePart((ReflectionComposablePartDefinition)null);
            });
        }

        [TestMethod]
        public void Constructor2_NullAsAttributedPartArgument_ShouldThrowArgumentNull()
        {
            ExceptionAssert.ThrowsArgument<ArgumentNullException>("attributedPart", () =>
            {
                new ReflectionComposablePart(PartDefinitionFactory.CreateAttributed(), (object)null);
            });
        }

        [TestMethod]
        public void Constructor2_ValueTypeAsAttributedPartArgument_ShouldThrowArgument()
        {
            ExceptionAssert.ThrowsArgument<ArgumentException>("attributedPart", () =>
            {
                new ReflectionComposablePart(PartDefinitionFactory.CreateAttributed(), 42);
            });
        }

        [TestMethod]
        public void Constructor1_AttributedComposablePartDefintion_ShouldProduceValidObject()
        {
            var definition = PartDefinitionFactory.CreateAttributed(typeof(MyExport));
            var part = new ReflectionComposablePart(definition);

            Assert.AreEqual(definition, part.Definition);
            Assert.IsNotNull(part.Metadata);

            Assert.IsTrue(part.IsInstanceLifetimeOwner, "Should take ownership of objects created from the type");
        }

        [TestMethod]
        public void Constructor1_Type_ShouldProduceValidObject()
        {
            var part = new ReflectionComposablePart(PartDefinitionFactory.CreateAttributed(typeof(MyExport)));
            Assert.IsTrue(part.IsInstanceLifetimeOwner, "Should take ownership of objects created from the type defintion");
        }

        [TestMethod]
        public void Constructor1_Object_ShouldProduceValidObject()
        {
            var part = new ReflectionComposablePart(PartDefinitionFactory.CreateAttributed(typeof(MyExport)), new MyExport());
            Assert.IsFalse(part.IsInstanceLifetimeOwner, "Ownership needs to be explicitly set when working with object references");
        }

        [TestMethod]
        public void Metadata_WhenDisposed_ShouldThrowObjectDisposed()
        {
            var part = CreateDefaultPart();
            part.Dispose();

            ExceptionAssert.ThrowsDisposed(part, () =>
            {
                var metadata = part.Metadata;
            });
        }

        [TestMethod]
        public void ImportDefinitions_WhenDisposed_ShouldThrowObjectDisposed()
        {
            var part = CreateDefaultPart();
            part.Dispose();

            ExceptionAssert.ThrowsDisposed(part, () =>
            {
                var definitions = part.ImportDefinitions;
            });
        }

        [TestMethod]
        public void ExportDefinitions_WhenDisposed_ShouldThrowObjectDisposed()
        {
            var part = CreateDefaultPart();
            part.Dispose();

            ExceptionAssert.ThrowsDisposed(part, () =>
            {
                var definitions = part.ExportDefinitions;
            });
        }

        [TestMethod]
        public void Dispose_IsInstanceLifetimeFalse_ShouldNotDisposeObject()
        {
            var disposable = new DisposableExportClass();
            var part = CreatePart(disposable);

            part.Dispose();

            Assert.IsFalse(part.IsInstanceLifetimeOwner);
            Assert.IsFalse(disposable.IsDisposed);
        }

        [TestMethod]
        public void Dispose_IsInstanceLifetimeTrue_ShouldDisposeObject()
        {
            var disposable = new DisposableExportClass();
            var part = CreatePart(disposable);
            part.IsInstanceLifetimeOwner = true;

            part.Dispose();

            Assert.IsTrue(part.IsInstanceLifetimeOwner);
            Assert.IsTrue(disposable.IsDisposed);
        }

        [TestMethod]
        public void RequiresDisposal_NonDisposableType_ShouldNotRequireDisposal()
        {
            var part = CreatePart(typeof(object));

            Assert.IsFalse(part.RequiresDisposal);
        }

        [TestMethod]
        public void RequiresDisposal_DisposableType_ShouldRequireDisposal()
        {
            var part = CreatePart(typeof(DisposableExportClass));

            Assert.IsTrue(part.RequiresDisposal);
        }

        [TestMethod]
        public void RequiresDisposal_NonDisposableInstance_ShouldNotRequireDisposal()
        {
            var part = CreatePart(new object());

            Assert.IsFalse(part.RequiresDisposal);
        }

        [TestMethod]
        public void RequiresDisposal_DisposableInstance_ShouldRequireDisposal()
        {
            var part = CreatePart(new DisposableExportClass());

            Assert.IsFalse(part.IsInstanceLifetimeOwner);
            Assert.IsFalse(part.RequiresDisposal, "Isn't the instance owner so shouldn't require disposal!");
        }

        [TestMethod]
        public void OnComposed_WhenDisposed_ShouldThrowObjectDisposed()
        {
            var part = CreateDefaultPart();
            part.Dispose();

            ExceptionAssert.ThrowsDisposed(part, () =>
            {
                part.OnComposed();
            });
        }

        [TestMethod]
        public void OnComposed_MissingPostImportsOnInstance_ShouldThrowComposition()
        {
            var part = CreatePart(new MySharedPartExport());

            // Dev10:484204 - This used to cause a failure but after we made 
            // ReflectionComposablePart internal we needed to back remove this 
            // validation for post imports to make declarative composition work.
            //part.OnComposed().VerifyFailure(CompositionIssueId.ImportNotSetOnPart);
            part.OnComposed();
        }

        [TestMethod]
        public void OnComposed_ProperlyComposed_ShouldSucceed()
        {
            var import = new TrivialImporter();
            var export = new TrivialExporter();

            var part = CreatePart(import);

            var importDef = part.ImportDefinitions.First();
            part.SetImport(importDef, CreateSimpleExports(export));
            part.OnComposed();
            Assert.IsTrue(export.done, "OnImportsSatisfied should have been called");
        }

        [TestMethod]
        public void OnComposed_UnhandledExceptionThrowInOnImportsSatisfied_ShouldThrowComposablePart()
        {
            var part = CreatePart(typeof(ExceptionDuringINotifyImport));
            var definition = part.ImportDefinitions.First();
            part.SetImport(definition, CreateSimpleExports(21));

            CompositionAssert.ThrowsPart<NotImplementedException>(ErrorId.ReflectionModel_PartOnImportsSatisfiedThrewException, RetryMode.DoNotRetry, () =>
            {
                part.OnComposed();
            });
        }

        [TestMethod]
        public void SetImport_WhenDisposed_ShouldThrowObjectDisposed()
        {
            var part = CreatePartWithNonRecomposableImport();
            var definition = part.ImportDefinitions.First();

            part.Dispose();

            ExceptionAssert.ThrowsDisposed(part, () =>
            {
                part.SetImport(definition, Enumerable.Empty<Export>());
            });
        }

        [TestMethod]
        public void SetImport_NullAsImportDefinitionArgument_ShouldThrowArgumentNull()
        {
            var part = CreateDefaultPart();

            ExceptionAssert.ThrowsArgument<ArgumentNullException>("definition", () =>
            {
                part.SetImport((ImportDefinition)null, Enumerable.Empty<Export>());
            });
        }

        [TestMethod]
        public void SetImport_NullAsExportsArgument_ShouldThrowArgumentNull()
        {
            var part = CreatePart(typeof(MySharedPartExport));
            var import = part.ImportDefinitions.First();

            ExceptionAssert.ThrowsArgument<ArgumentNullException>("exports", () =>
            {
                part.SetImport(import, (IEnumerable<Export>)null);
            });
        }

        [TestMethod]
        public void SetImport_ExportsArrayWithNullElementAsExportsArgument_ShouldThrowArgument()
        {
            var part = CreatePart(typeof(MySharedPartExport));
            var definition = part.ImportDefinitions.First();

            ExceptionAssert.ThrowsArgument<ArgumentException>("exports", () =>
            {
                part.SetImport(definition, new Export[] { null });
            });
        }

        [TestMethod]
        public void SetImport_WrongDefinitionAsDefinitionArgument_ShouldThrowArgument()
        {
            var part = CreateDefaultPart();

            var definition = ImportDefinitionFactory.Create();

            ExceptionAssert.ThrowsArgument<ArgumentException>("definition", () =>
            {
                part.SetImport(definition, Enumerable.Empty<Export>());
            });
        }

        [TestMethod]
        public void SetImport_SetNonRecomposableDefinitionAsDefinitionArgumentAfterOnComposed_ShouldThrowInvalidOperation()
        {
            var part = CreatePartWithNonRecomposableImport();
            var definition = part.ImportDefinitions.First();

            part.SetImport(definition, Enumerable.Empty<Export>());
            part.OnComposed();

            ExceptionAssert.Throws<InvalidOperationException>(() =>
            {
                part.SetImport(definition, Enumerable.Empty<Export>());
            });
        }

        [TestMethod]
        public void SetImport_ZeroOrOneDefinitionAsDefinitionArgumentAndTwoExportsAsExportsArgument_ShouldThrowArgument()
        {
            var part = CreatePartWithZeroOrOneImport();
            var definition = part.ImportDefinitions.First();

            var exports = ExportFactory.Create("Import", 2);

            ExceptionAssert.ThrowsArgument<ArgumentException>("exports", () =>
            {
                part.SetImport(definition, exports);
            });
        }

        [TestMethod]
        public void SetImport_ExactlyOneDefinitionAsDefinitionArgumentAndTwoExportsAsExportsArgument_ShouldThrowArgument()
        {
            var part = CreatePartWithExactlyOneImport();
            var definition = part.ImportDefinitions.First();

            var exports = ExportFactory.Create("Import", 2);

            ExceptionAssert.ThrowsArgument<ArgumentException>("exports", () =>
            {
                part.SetImport(definition, exports);
            });
        }

        [TestMethod]
        public void SetImport_ExactlyOneDefinitionAsDefinitionArgumentAndEmptyExportsAsExportsArgument_ShouldThrowArgument()
        {
            var part = CreatePartWithExactlyOneImport();
            var definition = part.ImportDefinitions.First();

            var exports = Enumerable.Empty<Export>();

            ExceptionAssert.ThrowsArgument<ArgumentException>("exports", () =>
            {
                part.SetImport(definition, exports);
            });
        }

        [TestMethod]
        public void SetImport_WrongTypeExportGiven_ShouldThrowComposablePart()
        {
            var part = CreatePart(new MySharedPartExport());
            var import = part.ImportDefinitions.First();

            CompositionAssert.ThrowsPart(ErrorId.ReflectionModel_ImportNotAssignableFromExport, () =>
            {
                part.SetImport(import, CreateSimpleExports("21"));
            });
        }

        [TestMethod]
        public void SetImport_SetPostValueAndSetAgainOnInstance_ShouldSetProperty()
        {
            var import = new MySharedPartExport();
            var part = CreatePart(import);
            var importDef = part.ImportDefinitions.First();

            part.SetImport(importDef, CreateSimpleExports(21));

            Assert.AreNotEqual(import.Value, 21, "Value should NOT be set on live object until OnComposed");
            part.OnComposed();

            Assert.AreEqual(import.Value, 21, "Value should be set on live object now");

            part.SetImport(importDef, CreateSimpleExports(42));

            Assert.AreNotEqual(import.Value, 42, "Value should NOT be rebound on live object");

            part.OnComposed();

            Assert.AreEqual(import.Value, 42, "Value should be set on live object now");
        }

        [TestMethod]
        public void GetExportedObject_WhenDisposed_ShouldThrowObjectDisposed()
        {
            var part = CreatePartWithExport();
            var definition = part.ExportDefinitions.First();

            part.Dispose();

            ExceptionAssert.ThrowsDisposed(part, () =>
            {
                part.GetExportedObject(definition);
            });
        }

        [TestMethod]
        public void GetExportedObject_NullAsDefinitionArgument_ShouldThrowArgumentNull()
        {
            var part = CreateDefaultPart();

            ExceptionAssert.ThrowsArgument<ArgumentNullException>("definition", () =>
            {
                part.GetExportedObject((ExportDefinition)null);
            });
        }

        [TestMethod]
        public void GetExportedObject_WrongDefinitionAsDefinitionArgument_ShouldThrowArgument()
        {
            var part = CreateDefaultPart();
            var definition = ExportDefinitionFactory.Create();

            ExceptionAssert.ThrowsArgument<ArgumentException>("definition", () =>
            {
                part.GetExportedObject(definition);
            });
        }

        [TestMethod]
        public void GetExportedObject_MissingPrerequisiteImport_ShouldThrowInvalidOperation()
        {
            var part = CreatePart(typeof(SimpleConstructorInjectedObject));
            var definition = part.ExportDefinitions.First();

            ExceptionAssert.Throws<InvalidOperationException>(() =>
            {
                part.GetExportedObject(definition);
            });
        }

        [Ignore]
        [TestMethod]
        [WorkItem(484204)]
        public void GetExportedObject_MissingPostImports_ShouldThrowComposition()
        {
            var part = CreatePart(typeof(MySharedPartExport));

            // Signal that the composition should be finished
            part.OnComposed();

            var definition = part.ExportDefinitions.First();

            // Dev10:484204 - This used to cause a failure but after we made 
            // ReflectionComposablePart internal we needed to back remove this 
            // validation for post imports to make declarative composition work.
            CompositionAssert.ThrowsError(ErrorId.ImportNotSetOnPart, () =>
            {
                part.GetExportedObject(definition);
            });
        }

        [TestMethod]
        public void GetExportedObject_NoConstructorOnDefinition_ShouldThrowComposablePart()
        {
            var part = CreatePart(typeof(ClassWithNoMarkedOrDefaultConstructor));

            var definition = part.ExportDefinitions.First();

            CompositionAssert.ThrowsPart(ErrorId.ReflectionModel_PartConstructorMissing, () =>
            {
                part.GetExportedObject(definition);
            });
        }

        [TestMethod]
        public void GetExportedObject_UnhandledExceptionThrowInConstructor_ShouldThrowComposablePart()
        {
            var part = CreatePart(typeof(ExportWithExceptionDuringConstruction));

            var definition = part.ExportDefinitions.First();

            CompositionAssert.ThrowsPart<NotImplementedException>(ErrorId.ReflectionModel_PartConstructorThrewException, () =>
            {
                part.GetExportedObject(definition);
            });
        }

        [TestMethod]
        public void GetExportedObject_GetObjectAfterSetPreImport_ShouldGetValue()
        {
            var part = CreatePart(typeof(SimpleConstructorInjectedObject));

            var import = part.ImportDefinitions.First();
            part.SetImport(import, CreateSimpleExports(21));

            part.OnComposed();

            var definition = part.ExportDefinitions.First();
            var exportObject = (SimpleConstructorInjectedObject)part.GetExportedObject(definition);

            Assert.AreEqual(21, exportObject.CISimpleValue);
        }

        [TestMethod]
        public void GetExportedObject_GetObjectAfterSetPostImport_ShouldGetValue()
        {
            var part = CreatePart(typeof(MySharedPartExport));

            var import = part.ImportDefinitions.First();
            part.SetImport(import, CreateSimpleExports(21));

            part.OnComposed();

            var definition = part.ExportDefinitions.First();
            var exportObject = (MySharedPartExport)part.GetExportedObject(definition);

            Assert.IsNotNull(exportObject);
            Assert.AreEqual(21, exportObject.Value);
        }

        [TestMethod]
        public void GetExportedObject_CallMultipleTimes_ShouldReturnSame()
        {
            var part = CreatePart(typeof(MySharedPartExport));

            var import = part.ImportDefinitions.First();
            part.SetImport(import, CreateSimpleExports(21));

            part.OnComposed();

            var definition = part.ExportDefinitions.First();
            var exportedObject1 = part.GetExportedObject(definition);
            var exportedObject2 = part.GetExportedObject(definition);

            Assert.AreSame(exportedObject1, exportedObject2);
        }

        [TestMethod]
        public void GetExportedObject_FromStaticClass_ShouldReturnExport()
        {
            var part = CreatePart(typeof(StaticExportClass));

            var definition = part.ExportDefinitions.First();

            var exportObject = (string)part.GetExportedObject(definition);

            Assert.AreEqual("StaticString", exportObject);
        }

        [TestMethod]
        public void GetExportedObject_OptionalPostNotGiven_ShouldReturnValidObject()
        {
            var part = CreatePart(typeof(ClassWithOptionalPostImport));
            part.OnComposed();

            var definition = part.ExportDefinitions.First();
            var exportObject = (ClassWithOptionalPostImport)part.GetExportedObject(definition);

            Assert.IsNull(exportObject.Formatter);
        }

        [TestMethod]
        public void GetExportedObject_OptionalPreNotGiven_ShouldReturnValidObject()
        {
            var part = CreatePart(typeof(ClassWithOptionalPreImport));
            part.OnComposed();

            var definition = part.ExportDefinitions.First();

            var exportedObject = (ClassWithOptionalPreImport)part.GetExportedObject(definition);
            Assert.IsNull(exportedObject.Formatter);
        }

        [TestMethod]
        public void ICompositionElementDisplayName_ShouldReturnTypeDisplayName()
        {
            var expectations = Expectations.GetAttributedTypes();
            foreach (var e in expectations)
            {
                var part = (ICompositionElement)CreatePart(e);

                Assert.AreEqual(e.GetDisplayName(), part.DisplayName);
            }
        }

        [TestMethod]
        public void ToString_ShouldReturnICompositionElementDisplayName()
        {
            var expectations = Expectations.GetAttributedTypes();
            foreach (var e in expectations)
            {
                var part = (ICompositionElement)CreatePart(e);

                Assert.AreEqual(part.DisplayName, part.ToString());
            }
        }

        [PartNotDiscoverable]
        public class MethodWithoutContractName
        {
            [Export]
            public void MethodWithoutContractNameNotAllowed()
            {
            }
        }

        [PartNotDiscoverable]
        public class ImportWithMultipleImports
        {
            [Import]
            [ImportMany]
            public int ImportWithMultipleImportAttributes { get; set; }
        }

        [TestMethod]
        public void ImportDefinitions_ImportWithMultipleImportAttributes_ShouldThrowInvalidOperation()
        {
            var part = CreatePart(typeof(ImportWithMultipleImports));

            // Dev10:602872 - At some point when we have a method for handling discovery errors we 
            // should move this error to that mechanism.

            ExceptionAssert.Throws<InvalidOperationException>(RetryMode.DoNotRetry, () =>
                {
                    part.ImportDefinitions.Count(); // Call count just to materialize the definitions
                });
        }

        [PartNotDiscoverable]
        public class ImportWithMultipleImportsInConstructor
        {
            [ImportingConstructor]
            public ImportWithMultipleImportsInConstructor([Import, ImportMany] int importWithMultipleImportAttributes)
            {
            }
        }

        [TestMethod]
        public void ImportDefinitions_ImportWithMultipleImportAttributesInConstructor_ShouldThrowInvalidOperation()
        {
            var part = CreatePart(typeof(ImportWithMultipleImportsInConstructor));

            // Dev10:602872 - At some point when we have a method for handling discovery errors we 
            // should move this error to that mechanism.

            ExceptionAssert.Throws<InvalidOperationException>(RetryMode.DoNotRetry, () =>
            {
                part.ImportDefinitions.Count(); // Call count just to materialize the definitions
            });
        }

        private Export[] CreateSimpleExports(object value)
        {
            var export = ExportFactory.Create("NoContract", () => value);

            return new Export[] { export };
        }

        private ReflectionComposablePart CreatePartWithExport()
        {
            return CreatePart(typeof(StaticExportClass));
        }

        private ReflectionComposablePart CreatePartWithNonRecomposableImport()
        {
            return CreatePart(typeof(SingleImportWithAllowDefault));
        }

        private ReflectionComposablePart CreatePartWithZeroOrOneImport()
        {
            return CreatePart(typeof(SingleImportWithAllowDefault));
        }

        private ReflectionComposablePart CreatePartWithExactlyOneImport()
        {
            return CreatePart(typeof(SingleImport));
        }

        private ReflectionComposablePart CreateDefaultPart()
        {
            return CreatePart(new object());
        }

        private ReflectionComposablePart CreatePart(object instance)
        {
            if (instance is Type)
            {
                var definition = PartDefinitionFactory.CreateAttributed((Type)instance);

                return (ReflectionComposablePart)definition.CreatePart();
            }
            else
            {
                var definition = PartDefinitionFactory.CreateAttributed(instance.GetType()); 

                return new ReflectionComposablePart(definition, instance);
            }
        }
    }
}
