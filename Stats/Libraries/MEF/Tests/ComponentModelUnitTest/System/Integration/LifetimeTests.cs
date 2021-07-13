// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Factories;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.UnitTesting;
using System.Linq;
using System.Reflection;
using System.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Integration
{
    [TestClass]
    public class LifetimeTests
    {
        [Export]
        public class AnyPartSimple
        {

        }

        [Export]
        public class AnyPartDisposable : IDisposable
        {
            public bool IsDisposed { get; set; }
            public void Dispose()
            {
                Assert.IsFalse(IsDisposed);
                IsDisposed = true;
            }
        }

        [Export]
        public class AnyPartRecomposable
        {
            [Import("Value", AllowRecomposition = true)]
            public int Value { get; set; }
        }

        [Export]
        public class AnyPartDisposableRecomposable : IDisposable
        {
            [Import("Value", AllowRecomposition = true)]
            public int Value { get; set; }

            public bool IsDisposed { get; set; }
            public void Dispose()
            {
                Assert.IsFalse(IsDisposed);
                IsDisposed = true;
            }
        }

        [TestMethod]
        public void PartAddedViaAddExportedObject_ShouldNotBeDisposedWithContainer()
        {
            var container = new CompositionContainer();
            var disposablePart = new AnyPartDisposable();
            var batch = new CompositionBatch();
            batch.AddPart(batch);
            container.Compose(batch);

            container.Dispose();
            Assert.IsFalse(disposablePart.IsDisposed);
        }

        [TestMethod]
        public void PartAddedTwice_AppearsTwice()
        {
            //  You probably shouldn't be adding a part to the container twice, but it's not something we're going to check for and throw an exception on
            var container = new CompositionContainer();
            var disposable = new AnyPartDisposable();
            var part = AttributedModelServices.CreatePart(disposable);
            var batch = new CompositionBatch();
            batch.AddPart(part);
            container.Compose(batch);

            batch = new CompositionBatch();
            batch.AddPart(part);
            container.Compose(batch);

            var exports = container.GetExports<AnyPartDisposable>();
            Assert.AreEqual(2, exports.Count);

            container.Dispose();
        }

        [TestMethod]
        public void AnyPart_Simple_ShouldNotBeCollected()
        {
            var catalog = new TypeCatalog(typeof(AnyPartSimple));
            var container = new CompositionContainer(catalog);

            var refTracker = new ReferenceTracker();

            refTracker.AddReferencesNotExpectedToBeCollected(
                container.GetExportedObject<AnyPartSimple>());

            refTracker.CollectAndAssert();

            GC.KeepAlive(container);
        }

        [TestMethod]
        public void AnyPart_Disposable_ShouldNotBeCollected()
        {
            var catalog = new TypeCatalog(typeof(AnyPartDisposable));
            var container = new CompositionContainer(catalog);

            var refTracker = new ReferenceTracker();

            refTracker.AddReferencesNotExpectedToBeCollected(
                container.GetExportedObject<AnyPartDisposable>());

            GC.KeepAlive(container);
        }

        [TestMethod]
        public void AnyPart_Disposable_ShouldBeDisposedWithContainer()
        {
            var catalog = new TypeCatalog(typeof(AnyPartDisposable));
            var container = new CompositionContainer(catalog);

            var exportedObject = container.GetExportedObject<AnyPartDisposable>();

            Assert.IsFalse(exportedObject.IsDisposed);

            container.Dispose();

            Assert.IsTrue(exportedObject.IsDisposed, "AnyPart should be disposed with the container!");
        }

        [TestMethod]
        public void AnyPart_RecomposabeImport_ShouldNotBeCollected()
        {
            var catalog = new TypeCatalog(typeof(AnyPartRecomposable));
            var container = new CompositionContainer(catalog);

            // Setup dependency
            CompositionBatch batch = new CompositionBatch();
            var valueKey = batch.AddExportedObject("Value", 21);
            container.Compose(batch);
            batch = null;

            var refTracker = new ReferenceTracker();

            refTracker.AddReferencesNotExpectedToBeCollected(
                container.GetExportedObject<AnyPartRecomposable>());
            refTracker.CollectAndAssert();

            // Lets make sure recomposition doesn't blow anything up here.
            batch = new CompositionBatch();
            batch.RemovePart(valueKey);
            batch.AddExportedObject("Value", 42);
            container.Compose(batch);
            batch = null;

            var exportedObject = (AnyPartRecomposable)refTracker.ReferencesNotExpectedToBeCollected[0].Target;
            Assert.AreEqual(42, exportedObject.Value);

            GC.KeepAlive(container);
        }

        [TestMethod]
        public void AnyPart_DisposableRecomposabeImport_ShouldNotBeCollected()
        {
            var catalog = new TypeCatalog(typeof(AnyPartDisposableRecomposable));
            var container = new CompositionContainer(catalog);

            // Setup dependency
            CompositionBatch batch = new CompositionBatch();
            var valueKey = batch.AddExportedObject("Value", 21);
            container.Compose(batch);
            batch = null;

            var refTracker = new ReferenceTracker();

            refTracker.AddReferencesNotExpectedToBeCollected(
                container.GetExportedObject<AnyPartDisposableRecomposable>());

            refTracker.CollectAndAssert();

            // Lets make sure recomposition doesn't blow anything up here.
            batch = new CompositionBatch();
            batch.RemovePart(valueKey);
            batch.AddExportedObject("Value", 42);
            container.Compose(batch);
            batch = null;

            var exportedObject = (AnyPartDisposableRecomposable)refTracker.ReferencesNotExpectedToBeCollected[0].Target;
            Assert.AreEqual(42, exportedObject.Value);

            GC.KeepAlive(container);

            container.Dispose();

            Assert.IsTrue(exportedObject.IsDisposed, "Any parts should be disposed with the container!");
        }

        [Export]
        [PartCreationPolicy(CreationPolicy.Shared)]
        public class SharedPartSimple
        {

        }

        [Export]
        [PartCreationPolicy(CreationPolicy.Shared)]
        public class SharedPartDisposable : IDisposable
        {
            public bool IsDisposed { get; set; }
            public void Dispose()
            {
                Assert.IsFalse(IsDisposed);
                IsDisposed = true;
            }
        }

        [Export]
        [PartCreationPolicy(CreationPolicy.Shared)]
        public class SharedPartRecomposable
        {
            [Import("Value", AllowRecomposition = true)]
            public int Value { get; set; } 
        }

        [Export]
        [PartCreationPolicy(CreationPolicy.Shared)]
        public class SharedPartDisposableRecomposable : IDisposable
        {
            [Import("Value", AllowRecomposition = true)]
            public int Value { get; set; }

            public bool IsDisposed { get; set; }
            public void Dispose()
            {
                Assert.IsFalse(IsDisposed);
                IsDisposed = true;
            }
        }

        [TestMethod]
        public void SharedPart_Simple_ShouldNotBeCollected()
        {
            var catalog = new TypeCatalog(typeof(SharedPartSimple));
            var container = new CompositionContainer(catalog);

            var refTracker = new ReferenceTracker();

            refTracker.AddReferencesNotExpectedToBeCollected(
                container.GetExportedObject<SharedPartSimple>());

            refTracker.CollectAndAssert();

            GC.KeepAlive(container);
        }

        [TestMethod]
        public void SharedPart_Disposable_ShouldNotBeCollected()
        {
            var catalog = new TypeCatalog(typeof(SharedPartDisposable));
            var container = new CompositionContainer(catalog);

            var refTracker = new ReferenceTracker();

            refTracker.AddReferencesNotExpectedToBeCollected(
                container.GetExportedObject<SharedPartDisposable>());

            refTracker.CollectAndAssert();

            GC.KeepAlive(container);
        }

        [TestMethod]
        public void SharedPart_Disposable_ShouldBeDisposedWithContainer()
        {
            var catalog = new TypeCatalog(typeof(SharedPartDisposable));
            var container = new CompositionContainer(catalog);

            var export = container.GetExportedObject<SharedPartDisposable>();

            Assert.IsFalse(export.IsDisposed);

            container.Dispose();

            Assert.IsTrue(export.IsDisposed, "SharedPart should be disposed with the container!");
        }
       
        [TestMethod]
        public void SharedPart_RecomposabeImport_ShouldNotBeCollected()
        {
            var catalog = new TypeCatalog(typeof(SharedPartRecomposable));
            var container = new CompositionContainer(catalog);

            // Setup dependency
            CompositionBatch batch = new CompositionBatch();
            var valueKey = batch.AddExportedObject("Value", 21);
            container.Compose(batch);
            batch = null;

            var refTracker = new ReferenceTracker();

            refTracker.AddReferencesNotExpectedToBeCollected(
                container.GetExportedObject<SharedPartRecomposable>());

            refTracker.CollectAndAssert();

            // Lets make sure recomposition doesn't blow anything up here.
            batch = new CompositionBatch();
            batch.RemovePart(valueKey);
            batch.AddExportedObject("Value", 42);
            container.Compose(batch);
            batch = null;

            var exportedObject = (SharedPartRecomposable)refTracker.ReferencesNotExpectedToBeCollected[0].Target;
            Assert.AreEqual(42, exportedObject.Value);

            GC.KeepAlive(container);
        }

        [TestMethod]
        public void SharedPart_DisposableRecomposabeImport_ShouldNotBeCollected()
        {
            var catalog = new TypeCatalog(typeof(SharedPartDisposableRecomposable));
            var container = new CompositionContainer(catalog);

            // Setup dependency
            CompositionBatch batch = new CompositionBatch();
            var valueKey = batch.AddExportedObject("Value", 21);
            container.Compose(batch);
            batch = null;

            var refTracker = new ReferenceTracker();

            refTracker.AddReferencesNotExpectedToBeCollected(
                container.GetExportedObject<SharedPartDisposableRecomposable>());

            refTracker.CollectAndAssert();

            // Lets make sure recomposition doesn't blow anything up here.
            batch = new CompositionBatch();
            batch.RemovePart(valueKey);
            batch.AddExportedObject("Value", 42);
            container.Compose(batch);
            batch = null;

            var exportedObject = (SharedPartDisposableRecomposable)refTracker.ReferencesNotExpectedToBeCollected[0].Target;
            Assert.AreEqual(42, exportedObject.Value);

            container.Dispose();

            Assert.IsTrue(exportedObject.IsDisposed, "Any parts should be disposed with the container!");
        }

        [Export]
        [PartCreationPolicy(CreationPolicy.NonShared)]
        public class NonSharedPartSimple
        {
            
        }

        [Export]
        [PartCreationPolicy(CreationPolicy.NonShared)]
        public class NonSharedPartRecomposable
        {
            [Import("Value", AllowRecomposition = true)]
            public int Value { get; set; }
        }

        [Export]
        [PartCreationPolicy(CreationPolicy.NonShared)]
        public class NonSharedPartDisposable : IDisposable
        {
            public bool IsDisposed { get; set; }
            public void Dispose()
            {
                Assert.IsFalse(IsDisposed);
                IsDisposed = true;
            }
        }

        [Export]
        [PartCreationPolicy(CreationPolicy.NonShared)]
        public class NonSharedPartDisposableRecomposable : IDisposable
        {
            private int _value;

            [Import("Value", AllowRecomposition = true)]
            public int Value
            {
                get
                {
                    if (this.IsDisposed) throw new ObjectDisposedException(this.GetType().Name);
                    return this._value; 
                }
                set
                {
                    if (this.IsDisposed) throw new ObjectDisposedException(this.GetType().Name);
                    this._value = value;
                }
            }

            public bool IsDisposed { get; set; }
            public void Dispose()
            {
                Assert.IsFalse(IsDisposed);
                IsDisposed = true;
            } 
        }

        [TestMethod]
        public void NonSharedPart_Simple_ShouldBeCollected()
        {
            var catalog = new TypeCatalog(typeof(NonSharedPartSimple));
            var container = new CompositionContainer(catalog);

            var refTracker = new ReferenceTracker();

            refTracker.AddReferencesExpectedToBeCollected(
                container.GetExportedObject<NonSharedPartSimple>());

            refTracker.CollectAndAssert();

            GC.KeepAlive(container);
        }

        [TestMethod]
        public void NonSharedPart_Disposable_ShouldNotBeCollected()
        {
            var catalog = new TypeCatalog(typeof(NonSharedPartDisposable));
            var container = new CompositionContainer(catalog);

            var refTracker = new ReferenceTracker();

            refTracker.AddReferencesNotExpectedToBeCollected(
                container.GetExportedObject<NonSharedPartDisposable>());

            refTracker.CollectAndAssert();

            GC.KeepAlive(container);
        }

        [TestMethod]
        public void NonSharedPart_Disposable_ShouldBeDisposedWithContainer()
        {
            var catalog = new TypeCatalog(typeof(NonSharedPartDisposable));
            var container = new CompositionContainer(catalog);

            var export = container.GetExportedObject<NonSharedPartDisposable>();

            Assert.IsFalse(export.IsDisposed);

            container.Dispose();

            Assert.IsTrue(export.IsDisposed, "NonSharedParts should be disposed with the container!");
        }

        [TestMethod]
        public void NonSharedPart_RecomposableImport_WithReference_ShouldNotBeCollected()
        {
            var catalog = new TypeCatalog(typeof(NonSharedPartRecomposable));
            var container = new CompositionContainer(catalog);

            // Setup dependency
            CompositionBatch batch = new CompositionBatch();
            var valueKey = batch.AddExportedObject("Value", 21);
            container.Compose(batch);
            batch = null;

            var exportedObject = container.GetExportedObject<NonSharedPartRecomposable>();

            var refTracker = new ReferenceTracker();

            refTracker.AddReferencesNotExpectedToBeCollected(exportedObject);

            refTracker.CollectAndAssert();

            // Recompose should work because we are still holding a reference to the exported object.
            batch = new CompositionBatch();
            batch.RemovePart(valueKey);
            batch.AddExportedObject("Value", 42);
            container.Compose(batch);
            batch = null;

            Assert.AreEqual(42, exportedObject.Value, "Value should have been recomposed");

            GC.KeepAlive(container);
        }

        [TestMethod]
        public void NonSharedPart_DisposableRecomposabeImport_NoReference_ShouldNotBeCollected()
        {
            var catalog = new TypeCatalog(typeof(NonSharedPartDisposableRecomposable));
            var container = new CompositionContainer(catalog);

            // Setup dependency
            CompositionBatch batch = new CompositionBatch();
            var valueKey = batch.AddExportedObject("Value", 21);
            container.Compose(batch);
            batch = null;

            var refTracker = new ReferenceTracker();

            refTracker.AddReferencesNotExpectedToBeCollected(
                container.GetExportedObject<NonSharedPartDisposableRecomposable>());

            refTracker.CollectAndAssert();

            // Recompose just to ensure we don't blow up, even though we don't expect anything to happen.
            batch = new CompositionBatch();
            batch.RemovePart(valueKey);
            batch.AddExportedObject("Value", 42);
            container.Compose(batch);
            batch = null;

            var exportedObject = (NonSharedPartDisposableRecomposable)refTracker.ReferencesNotExpectedToBeCollected[0].Target;
            Assert.AreEqual(42, exportedObject.Value, "Value shoudl ahve been recomposed.");
            
            GC.KeepAlive(container);
        }

        [Export]
        public class SharedState
        {
            public static int instanceNumber = 0;
            public SharedState()
            {
                MyInstanceNumber = instanceNumber++;
            }

            public int MyInstanceNumber { get; private set; }
        }

        [PartCreationPolicy(CreationPolicy.NonShared)]
        public class NonSharedState
        {
            [Import(AllowRecomposition = true)]
            public SharedState State { set { ExportState = value; } }

            [Export("SharedFromNonShared")]
            public SharedState ExportState { get; private set; }
        }

        [TestMethod]
        public void NonSharedPart_TwoRecomposablePartsSameExportedObject()
        {
            // This test is primarily used to ensure that we allow for multiple parts to be associated
            // with the same exported object.
            var catalog = new TypeCatalog(typeof(SharedState), typeof(NonSharedState));
            var container = new CompositionContainer(catalog);

            var export1 = container.GetExportedObject<SharedState>("SharedFromNonShared");
            var export2 = container.GetExportedObject<SharedState>("SharedFromNonShared");

            // Same exported object that comes from two different recomposable part instances.
            Assert.AreEqual(export1.MyInstanceNumber, export2.MyInstanceNumber, "Should be the same shared object!");
        }

        [Export]
        [PartCreationPolicy(CreationPolicy.NonShared)]
        public class SharedImporter
        {
            [Import(RequiredCreationPolicy = CreationPolicy.Shared)]
            public AnyPartSimple AnyPartSimple { get; set; }

            [Import(RequiredCreationPolicy = CreationPolicy.Shared)]
            public AnyPartDisposable AnyPartDisposable { get; set; }

            [Import(RequiredCreationPolicy = CreationPolicy.Shared)]
            public AnyPartRecomposable AnyPartRecomposable { get; set; }

            [Import(RequiredCreationPolicy = CreationPolicy.Shared)]
            public AnyPartDisposableRecomposable AnyPartDisposableRecomposable { get; set; }
        }

        [Export]
        [PartCreationPolicy(CreationPolicy.NonShared)]
        public class NonSharedImporter
        {
            [Import(RequiredCreationPolicy = CreationPolicy.NonShared)]
            public AnyPartSimple AnyPartSimple { get; set; }

            [Import(RequiredCreationPolicy = CreationPolicy.NonShared)]
            public AnyPartDisposable AnyPartDisposable { get; set; }

            [Import(RequiredCreationPolicy = CreationPolicy.NonShared)]
            public AnyPartRecomposable AnyPartRecomposable { get; set; }

            [Import(RequiredCreationPolicy = CreationPolicy.NonShared)]
            public AnyPartDisposableRecomposable AnyPartDisposableRecomposable { get; set; }
        }

        private static CompositionContainer GetContainer()
        {
            var container = ContainerFactory.CreateWithAttributedCatalog(
                typeof(LifetimeTests).GetNestedTypes(BindingFlags.Public));
            CompositionBatch batch = new CompositionBatch();
            batch.AddExportedObject("Value", 21);
            container.Compose(batch);
            return container;
        }

        [TestMethod]
        public void GetReleaseExport_SharedRoot_ShouldNotDisposeChain()
        {
            var container = GetContainer();

            var export = container.GetExport<SharedImporter>();
            var exportedObject = export.GetExportedObject();

            container.ReleaseExport(export);

            Assert.IsFalse(exportedObject.AnyPartDisposable.IsDisposed);
            Assert.IsFalse(exportedObject.AnyPartDisposableRecomposable.IsDisposed);
        }

        [TestMethod]
        public void GetReleaseExport_SharedPart_ShouldCollectOnlyRoot()
        {
            var container = GetContainer();

            var export = container.GetExport<SharedImporter>();
            var exportedObject = export.GetExportedObject();

            container.ReleaseExport(export);

            var refTracker = new ReferenceTracker();

            refTracker.AddReferencesExpectedToBeCollected(
                exportedObject);

            refTracker.AddReferencesNotExpectedToBeCollected(
                exportedObject.AnyPartDisposable,
                exportedObject.AnyPartDisposableRecomposable,
                exportedObject.AnyPartRecomposable,
                exportedObject.AnyPartSimple);

            export = null;
            exportedObject = null;

            refTracker.CollectAndAssert();

            GC.KeepAlive(container);
        }

        [TestMethod]
        public void AddRemovePart_SharedRoot_ShouldNotDisposeChain()
        {
            var container = GetContainer();

            var exportedObject = new SharedImporter();

            CompositionBatch batch = new CompositionBatch();
            var part = batch.AddPart(exportedObject);
            container.Compose(batch);

            batch = new CompositionBatch();
            batch.RemovePart(part);
            container.Compose(batch);

            Assert.IsFalse(exportedObject.AnyPartDisposable.IsDisposed);
            Assert.IsFalse(exportedObject.AnyPartDisposableRecomposable.IsDisposed);
        }

        [TestMethod]
        public void AddRemovePart_SharedPart_ShouldCollectOnlyRoot()
        {
            var container = GetContainer();
            
            var exportedObject = new SharedImporter();

            CompositionBatch batch = new CompositionBatch();
            var part = batch.AddPart(exportedObject);
            container.Compose(batch);
            batch = null;

            batch = new CompositionBatch();
            batch.RemovePart(part);
            container.Compose(batch);
            batch = null;

            var refTracker = new ReferenceTracker();

            refTracker.AddReferencesExpectedToBeCollected(
             exportedObject);

            refTracker.AddReferencesNotExpectedToBeCollected(
                exportedObject.AnyPartDisposable,
                exportedObject.AnyPartDisposableRecomposable,
                exportedObject.AnyPartRecomposable,
                exportedObject.AnyPartSimple);

            part = null;
            exportedObject = null;

            refTracker.CollectAndAssert();

            GC.KeepAlive(container);
        }

        [TestMethod]
        public void ContainerDispose_SharedRoot_ShouldDisposeChain()
        {
            var container = GetContainer();

            var export = container.GetExport<SharedImporter>();
            var exportedObject = export.GetExportedObject();

            container.Dispose();

            Assert.IsTrue(exportedObject.AnyPartDisposable.IsDisposed);
            Assert.IsTrue(exportedObject.AnyPartDisposableRecomposable.IsDisposed);
        }

        [TestMethod]
        public void GetReleaseExport_NonSharedRoot_ShouldDisposeChain()
        {
            var container = GetContainer();

            var exports = new List<Export<NonSharedImporter>>();
            var exportedObjects = new List<NonSharedImporter>();

            // Executing this 100 times to help uncover any GC bugs
            for (int i = 0; i < 100; i++)
            {
                var export = container.GetExport<NonSharedImporter>();
                var exportedObject = export.GetExportedObject();

                exports.Add(export);
                exportedObjects.Add(exportedObject);
            }

            for (int i = 0; i < 100; i++)
            {
                var export = exports[i];
                var exportedObject = exportedObjects[i];

                container.ReleaseExport(export);

                Assert.IsTrue(exportedObject.AnyPartDisposable.IsDisposed);
                Assert.IsTrue(exportedObject.AnyPartDisposableRecomposable.IsDisposed);
            }
        }

        [TestMethod]
        public void ReleaseExports_ShouldDispose_NonSharedParts()
        {
            var container = GetContainer();

            var export1 = container.GetExport<NonSharedImporter>();
            var exportedObject1 = export1.GetExportedObject();

            var export2 = container.GetExport<NonSharedImporter>();
            var exportedObject2 = export2.GetExportedObject();

            container.ReleaseExports(new [] {export1, export2});

            Assert.IsTrue(exportedObject1.AnyPartDisposable.IsDisposed);
            Assert.IsTrue(exportedObject1.AnyPartDisposableRecomposable.IsDisposed);

            Assert.IsTrue(exportedObject2.AnyPartDisposable.IsDisposed);
            Assert.IsTrue(exportedObject2.AnyPartDisposableRecomposable.IsDisposed);
        }

        [TestMethod]
        public void GetReleaseExport_NonSharedPart_ShouldCollectWholeObjectChain()
        {
            var container = GetContainer();

            var export = container.GetExport<NonSharedImporter>();
            var exportedObject = export.GetExportedObject();

            container.ReleaseExport(export);

            var refTracker = new ReferenceTracker();

            refTracker.AddReferencesExpectedToBeCollected(
                exportedObject,
                exportedObject.AnyPartDisposable,
                exportedObject.AnyPartDisposableRecomposable,
                exportedObject.AnyPartRecomposable,
                exportedObject.AnyPartSimple);

            export = null;
            exportedObject = null;

            refTracker.CollectAndAssert();

            GC.KeepAlive(container);
        }

        [TestMethod]
        public void AddRemovePart_NonSharedRoot_ShouldDisposeChain()
        {
            var container = GetContainer();

            var exportedObject = new NonSharedImporter();

            CompositionBatch batch = new CompositionBatch();
            var part = batch.AddPart(exportedObject);
            container.Compose(batch);

            batch = new CompositionBatch();
            batch.RemovePart(part);
            container.Compose(batch);

            Assert.IsTrue(exportedObject.AnyPartDisposable.IsDisposed);
            Assert.IsTrue(exportedObject.AnyPartDisposableRecomposable.IsDisposed);
        }

        [TestMethod]
        public void AddRemovePart_NonSharedPart_ShouldCollectWholeObjectChain()
        {
            var container = GetContainer();

            var exportedObject = new NonSharedImporter();

            CompositionBatch batch = new CompositionBatch();
            var part = batch.AddPart(exportedObject);
            container.Compose(batch);
            batch = null;

            batch = new CompositionBatch();
            batch.RemovePart(part);
            container.Compose(batch);
            batch = null;

            var refTracker = new ReferenceTracker();

            refTracker.AddReferencesExpectedToBeCollected(
                exportedObject,
                exportedObject.AnyPartDisposable,
                exportedObject.AnyPartDisposableRecomposable,
                exportedObject.AnyPartRecomposable,
                exportedObject.AnyPartSimple);

            part = null;
            exportedObject = null;

            refTracker.CollectAndAssert();

            GC.KeepAlive(container);
        }

        [TestMethod]
        public void ContainerDispose_NonSharedRoot_ShouldNotDisposeChain()
        {
            var container = GetContainer();

            var export = container.GetExport<NonSharedImporter>();
            var exportedObject = export.GetExportedObject();

            container.Dispose();

            Assert.IsTrue(exportedObject.AnyPartDisposable.IsDisposed);
            Assert.IsTrue(exportedObject.AnyPartDisposableRecomposable.IsDisposed);
        }

        [TestMethod]
        public void GetReleaseExport_NonSharedPart_ShouldNotRecomposeAfterRelease()
        {
            var catalog = new TypeCatalog(typeof(NonSharedPartRecomposable));
            var container = new CompositionContainer(catalog);

            // Setup dependency
            CompositionBatch batch = new CompositionBatch();
            var valueKey = batch.AddExportedObject("Value", 21);
            container.Compose(batch);
            
            var export = container.GetExport<NonSharedPartRecomposable>();
            var exportedObject = export.GetExportedObject();

            Assert.AreEqual(21, exportedObject.Value);

            container.ReleaseExport(export);

            // Recompose just to ensure we don't blow up, even though we don't expect anything to happen.
            batch = new CompositionBatch();
            batch.RemovePart(valueKey);
            batch.AddExportedObject("Value", 42);
            container.Compose(batch);

            Assert.AreEqual(21, exportedObject.Value, "Value should not be recomposed after ReleaseExport is called on it.");
        }

        [TestMethod]
        public void GetExportManualDisposeThenRecompose_NonSharedDisposableRecomposablePart_ShouldThrowComposition()
        {
            var catalog = new TypeCatalog(typeof(NonSharedPartDisposableRecomposable));
            var container = new CompositionContainer(catalog);

            // Setup dependency
            CompositionBatch batch = new CompositionBatch();
            var valueKey = batch.AddExportedObject("Value", 21);
            container.Compose(batch);
            
            var export = container.GetExport<NonSharedPartDisposableRecomposable>();
            var exportedObject = export.GetExportedObject();

            Assert.AreEqual(21, exportedObject.Value);

            exportedObject.Dispose();

            // Recompose should cause a ObjectDisposedException.
            batch = new CompositionBatch();
            batch.RemovePart(valueKey);
            batch.AddExportedObject("Value", 42);

            CompositionAssert.ThrowsError(
                              ErrorId.CompositionEngine_PartCannotActivate,         // Cannot activate part because
                              ErrorId.ReflectionModel_ImportThrewException,         // Import threw an exception
                              RetryMode.DoNotRetry,
                              () => 
            {
                container.Compose(batch);
            });
        }
 
        [Export]
        public class MyImporter
        {
            [Import(AllowDefault = true, AllowRecomposition = true, RequiredCreationPolicy = CreationPolicy.NonShared)]
            public AnyPartDisposable AnyPartDisposable { get; set; }
        }

        [TestMethod]
        public void RecomposeCausesOldImportedValuesToBeDisposed()
        {
            var cat = new AggregateCatalog();
            var cat1 = new TypeCatalog(typeof(AnyPartDisposable));

            cat.Catalogs.Add(new TypeCatalog(typeof (MyImporter)));
            cat.Catalogs.Add(cat1);

            var container = new CompositionContainer(cat);

            var importer = container.GetExportedObject<MyImporter>();

            var anyPart = importer.AnyPartDisposable;

            Assert.IsFalse(anyPart.IsDisposed);
            Assert.IsInstanceOfType(anyPart, typeof(AnyPartDisposable));

            // Remove the instance of MyClass1
            cat.Catalogs.Remove(cat1);

            Assert.IsNull(importer.AnyPartDisposable);
            Assert.IsTrue(anyPart.IsDisposed);
        }

        private static CompositionContainer CreateParentChildContainerWithNonSharedImporter()
        {
            var parentCat = CatalogFactory.CreateAttributed(typeof(AnyPartDisposable),
                                      typeof(AnyPartDisposableRecomposable),
                                      typeof(AnyPartRecomposable),
                                      typeof(AnyPartSimple));
            var parent = new CompositionContainer(parentCat);
            CompositionBatch batch = new CompositionBatch();
            batch.AddExportedObject("Value", 21);
            parent.Compose(batch);

            var childCat = CatalogFactory.CreateAttributed(typeof(NonSharedImporter));
            var child = new CompositionContainer(childCat, parent);

            return child;
        }

        [TestMethod]
        public void ChildContainerGetReleaseExport_NonSharedRoot_ShouldDisposeChain()
        {
            var child = CreateParentChildContainerWithNonSharedImporter();

            var export = child.GetExport<NonSharedImporter>();
            var exportedObject = export.GetExportedObject();

            child.ReleaseExport(export);

            Assert.IsTrue(exportedObject.AnyPartDisposable.IsDisposed);
            Assert.IsTrue(exportedObject.AnyPartDisposableRecomposable.IsDisposed);
        }

        [TestMethod]
        public void ChildContainerGetReleaseExport_NonSharedPart_ShouldCollectWholeObjectChain()
        {
            var child = CreateParentChildContainerWithNonSharedImporter();

            var export = child.GetExport<NonSharedImporter>();
            var exportedObject = export.GetExportedObject();

            child.ReleaseExport(export);

            var refTracker = new ReferenceTracker();

            refTracker.AddReferencesExpectedToBeCollected(
                exportedObject,
                exportedObject.AnyPartDisposable,
                exportedObject.AnyPartDisposableRecomposable,
                exportedObject.AnyPartRecomposable,
                exportedObject.AnyPartSimple);

            export = null;
            exportedObject = null;

            refTracker.CollectAndAssert();

            GC.KeepAlive(child);
        }

        [TestMethod]
        public void ChildContainerAddRemovePart_NonSharedRoot_ShouldDisposeChain()
        {
            var child = CreateParentChildContainerWithNonSharedImporter();

            var exportedObject = new NonSharedImporter();

            CompositionBatch batch = new CompositionBatch();
            var part = batch.AddPart(exportedObject);
            child.Compose(batch);

            batch = new CompositionBatch();
            batch.RemovePart(part);
            child.Compose(batch);

            Assert.IsTrue(exportedObject.AnyPartDisposable.IsDisposed);
            Assert.IsTrue(exportedObject.AnyPartDisposableRecomposable.IsDisposed);
        }

        [TestMethod]
        public void ChildContainerAddRemovePart_NonSharedPart_ShouldCollectWholeObjectChain()
        {
            var child = CreateParentChildContainerWithNonSharedImporter();

            var exportedObject = new NonSharedImporter();

            CompositionBatch batch = new CompositionBatch();
            var part = batch.AddPart(exportedObject);
            child.Compose(batch);
            batch = null;

            batch = new CompositionBatch();
            batch.RemovePart(part);
            child.Compose(batch);
            batch = null;

            var refTracker = new ReferenceTracker();

            refTracker.AddReferencesExpectedToBeCollected(
                exportedObject,
                exportedObject.AnyPartDisposable,
                exportedObject.AnyPartDisposableRecomposable,
                exportedObject.AnyPartRecomposable,
                exportedObject.AnyPartSimple);

            part = null;
            exportedObject = null;

            refTracker.CollectAndAssert();

            GC.KeepAlive(child);
        }

        [TestMethod]
        public void ChildContainerAddRemovePart_NonSharedRoot_ShouldNotDisposeChain()
        {
            var child = CreateParentChildContainerWithNonSharedImporter();

            var exportedObject = child.GetExportedObject<NonSharedImporter>();

            child.Dispose();

            Assert.IsFalse(exportedObject.AnyPartDisposable.IsDisposed);
            Assert.IsFalse(exportedObject.AnyPartDisposableRecomposable.IsDisposed);
        }

#if CLR40
        [TestMethod]
        public void ContainerDispose_SharedPart_ShouldCollectWholeObjectChain()
        {
            // Test only works properly with while using the real ConditionalWeakTable
            var container = GetContainer();

            var export = container.GetExport<SharedImporter>();
            var exportedObject = export.GetExportedObject();

            container.Dispose();

            var refTracker = new ReferenceTracker();

            refTracker.AddReferencesExpectedToBeCollected(
                exportedObject,
                exportedObject.AnyPartDisposable,
                exportedObject.AnyPartDisposableRecomposable,
                exportedObject.AnyPartRecomposable,
                exportedObject.AnyPartSimple);

            export = null;
            exportedObject = null;

            refTracker.CollectAndAssert();

            GC.KeepAlive(container);
        }

        [TestMethod]
        public void ContainerDispose_NonSharedPart_ShouldCollectWholeObjectChain()
        {
            // Test only works properly with while using the real ConditionalWeakTable
            var container = GetContainer();

            var export = container.GetExport<NonSharedImporter>();
            var exportedObject = export.GetExportedObject();

            container.Dispose();

            var refTracker = new ReferenceTracker();

            refTracker.AddReferencesExpectedToBeCollected(
                exportedObject,
                exportedObject.AnyPartDisposable,
                exportedObject.AnyPartDisposableRecomposable,
                exportedObject.AnyPartRecomposable,
                exportedObject.AnyPartSimple);

            export = null;
            exportedObject = null;

            refTracker.CollectAndAssert();

            GC.KeepAlive(container);
        }

        [TestMethod]
        public void NonSharedImporter_ReleaseReference_ShouldCollectWholeChain()
        {
            var container = GetContainer();

            var export = container.GetExport<NonSharedImporter>();
            var exportedObject = export.GetExportedObject();

            var refTracker = new ReferenceTracker();

            // Non-Disposable references in the chain should be GC'ed
            refTracker.AddReferencesExpectedToBeCollected(
                exportedObject,
                exportedObject.AnyPartRecomposable,
                exportedObject.AnyPartSimple);

            // Disposable references in the chain should NOT be GC'ed
            refTracker.AddReferencesNotExpectedToBeCollected(
                exportedObject.AnyPartDisposable,
                exportedObject.AnyPartDisposableRecomposable);

            export = null;
            exportedObject = null;

            refTracker.CollectAndAssert();

            GC.KeepAlive(container);
        }

        [TestMethod]
        public void ChildContainerDispose_NonSharedPart_ShouldOnlyCleanupChildAndSimpleNonShared()
        {
            var child = CreateParentChildContainerWithNonSharedImporter();

            var exportedObject = child.GetExportedObject<NonSharedImporter>();

            child.Dispose();

            var refTracker = new ReferenceTracker();

            refTracker.AddReferencesExpectedToBeCollected(
                exportedObject,                // object in child
                exportedObject.AnyPartSimple,  // No reference parent so collected.
                exportedObject.AnyPartRecomposable); 

            // These are in the parent and will not be cleaned out
            refTracker.AddReferencesNotExpectedToBeCollected(
                exportedObject.AnyPartDisposable,
                exportedObject.AnyPartDisposableRecomposable);

            exportedObject = null;

            refTracker.CollectAndAssert();

            GC.KeepAlive(child);
        }

        [TestMethod]
        public void NonSharedPart_RecomposableImport_NoReference_ShouldBeCollected()
        {
            var catalog = new TypeCatalog(typeof(NonSharedPartRecomposable));
            var container = new CompositionContainer(catalog);

            // Setup dependency
            CompositionBatch batch = new CompositionBatch();
            var valueKey = batch.AddExportedObject("Value", 21);
            container.Compose(batch);
            batch = null;

            var refTracker = new ReferenceTracker();

            refTracker.AddReferencesExpectedToBeCollected(
                container.GetExportedObject<NonSharedPartRecomposable>());

            refTracker.CollectAndAssert();

            // Recompose just to ensure we don't blow up, even though we don't expect anything to happen.
            batch = new CompositionBatch();
            batch.RemovePart(valueKey);
            batch.AddExportedObject("Value", 42);
            container.Compose(batch);
            batch = null;

            GC.KeepAlive(container);
        }

        [TestMethod]
        public void ReleaseExports_ShouldWorkWithExportCollection()
        {
            var container = GetContainer();
            var exports = container.GetExports<NonSharedImporter>();

            Assert.IsTrue(exports.Count > 0);

            var exportedObjects = exports.Select(export => export.GetExportedObject()).ToList();

            container.ReleaseExports(exports);

            foreach (var obj in exportedObjects)
            {
                Assert.IsTrue(obj.AnyPartDisposable.IsDisposed);
                Assert.IsTrue(obj.AnyPartDisposableRecomposable.IsDisposed);
            }
        }
#endif
    }
}
