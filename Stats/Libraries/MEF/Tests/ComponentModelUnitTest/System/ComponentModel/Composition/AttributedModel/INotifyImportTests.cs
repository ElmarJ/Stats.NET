// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition.Factories;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.Composition.AttributedModel
{
    [TestClass()]
    public class INotifyImportTests
    {
        [TestMethod()]
        public void ImportCompletedTest()
        {
            var container = ContainerFactory.Create(); 
            CompositionBatch batch = new CompositionBatch();
            var entrypoint = new UpperCaseStringComponent();

            batch.AddParts(new LowerCaseString("abc"), entrypoint);
            container.Compose(batch);

            batch = new CompositionBatch();
            batch.AddParts(new object());
            container.Compose(batch);

            Assert.AreEqual(entrypoint.LowerCaseStrings.Count, 1);
            Assert.AreEqual(entrypoint.ImportCompletedCallCount, 1);
            Assert.AreEqual(entrypoint.UpperCaseStrings.Count, 1);
            Assert.AreEqual(entrypoint.LowerCaseStrings[0].GetExportedObject().String, "abc");
            Assert.AreEqual(entrypoint.UpperCaseStrings[0], "ABC");
        }

        [TestMethod]
        public void ImportCompletedWithRecomposing()
        {
            var container = ContainerFactory.Create(); 
            CompositionBatch batch = new CompositionBatch();
            var entrypoint = new UpperCaseStringComponent();

            batch.AddParts(new LowerCaseString("abc"), entrypoint);
            container.Compose(batch);

            Assert.AreEqual(entrypoint.LowerCaseStrings.Count, 1);
            Assert.AreEqual(entrypoint.ImportCompletedCallCount, 1);
            Assert.AreEqual(entrypoint.UpperCaseStrings.Count, 1);
            Assert.AreEqual(entrypoint.LowerCaseStrings[0].GetExportedObject().String, "abc");
            Assert.AreEqual(entrypoint.UpperCaseStrings[0], "ABC");

            // Add another component to verify recomposing
            batch = new CompositionBatch();
            batch.AddParts(new LowerCaseString("def"));
            container.Compose(batch);

            Assert.AreEqual(entrypoint.LowerCaseStrings.Count, 2);
            Assert.AreEqual(entrypoint.ImportCompletedCallCount, 2);
            Assert.AreEqual(entrypoint.UpperCaseStrings.Count, 2);
            Assert.AreEqual(entrypoint.LowerCaseStrings[1].GetExportedObject().String, "def");
            Assert.AreEqual(entrypoint.UpperCaseStrings[1], "DEF");

            // Verify that adding a random component doesn't cause
            // the OnImportsSatisfied to be called again.
            batch = new CompositionBatch();
            batch.AddParts(new object());
            container.Compose(batch);

            Assert.AreEqual(entrypoint.LowerCaseStrings.Count, 2);
            Assert.AreEqual(entrypoint.ImportCompletedCallCount, 2);
            Assert.AreEqual(entrypoint.UpperCaseStrings.Count, 2);
        }

        [TestMethod]
        public void ImportCompletedAddPartAndBindComponent()
        {
            var container = ContainerFactory.Create();
            CompositionBatch batch = new CompositionBatch();
            batch.AddParts(new CallbackImportNotify(delegate
            {
                batch = new CompositionBatch();
                batch.AddPart(new object());
                container.Compose(batch);
            }));

            container.Compose(batch);
        }

        [TestMethod]
        public void ImportCompletedChildNeedsParentContainer()
        {
            var cat = CatalogFactory.CreateDefaultAttributed();
            var parent = new CompositionContainer(cat);

            CompositionBatch parentBatch = new CompositionBatch();
            CompositionBatch childBatch = new CompositionBatch();
            CompositionBatch child2Batch = new CompositionBatch();

            parentBatch.AddExportedObject<ICompositionService>(parent);
            parent.Compose(parentBatch);
            var child = new CompositionContainer(parent);
            var child2 = new CompositionContainer(parent);
            
            var parentImporter = new MyNotifyImportImporter(parent);
            var childImporter = new MyNotifyImportImporter(child);
            var child2Importer = new MyNotifyImportImporter(child2);

            parentBatch = new CompositionBatch();
            parentBatch.AddPart(parentImporter);
            childBatch.AddPart(childImporter);
            child2Batch.AddPart(child2Importer);

            parent.Compose(parentBatch);
            child.Compose(childBatch);
            child2.Compose(child2Batch);
            

            Assert.AreEqual(1, parentImporter.ImportCompletedCallCount);
            Assert.AreEqual(1, childImporter.ImportCompletedCallCount);
            Assert.AreEqual(1, child2Importer.ImportCompletedCallCount);

            MyNotifyImportExporter parentExporter = parent.GetExportedObject<MyNotifyImportExporter>("MyNotifyImportExporter");
            Assert.AreEqual(1, parentExporter.ImportCompletedCallCount);

            MyNotifyImportExporter childExporter = child.GetExportedObject<MyNotifyImportExporter>("MyNotifyImportExporter");
            Assert.AreEqual(1, childExporter.ImportCompletedCallCount);

            MyNotifyImportExporter child2Exporter = child2.GetExportedObject<MyNotifyImportExporter>("MyNotifyImportExporter");
            Assert.AreEqual(1, child2Exporter.ImportCompletedCallCount);
        }

        [TestMethod]
        public void ImportCompletedChildDoesnotNeedParentContainer()
        {
            var cat = CatalogFactory.CreateDefaultAttributed();
            var parent = new CompositionContainer(cat);

            CompositionBatch parentBatch = new CompositionBatch();
            CompositionBatch childBatch = new CompositionBatch();

            parentBatch.AddExportedObject<ICompositionService>(parent);
            parent.Compose(parentBatch);

            var child = new CompositionContainer(parent);

            var parentImporter = new MyNotifyImportImporter(parent);
            var childImporter = new MyNotifyImportImporter(child);

            parentBatch = new CompositionBatch();
            parentBatch.AddPart(parentImporter);

            childBatch.AddParts(childImporter, new MyNotifyImportExporter());

            child.Compose(childBatch);

            Assert.AreEqual(0, parentImporter.ImportCompletedCallCount);
            Assert.AreEqual(1, childImporter.ImportCompletedCallCount);

            // Parent will become bound at this point.
            MyNotifyImportExporter parentExporter = parent.GetExportedObject<MyNotifyImportExporter>("MyNotifyImportExporter");
            parent.Compose(parentBatch);
            Assert.AreEqual(1, parentImporter.ImportCompletedCallCount);
            Assert.AreEqual(1, parentExporter.ImportCompletedCallCount);

            MyNotifyImportExporter childExporter = child.GetExportedObject<MyNotifyImportExporter>("MyNotifyImportExporter");
            Assert.AreEqual(1, childExporter.ImportCompletedCallCount);
        }

        [TestMethod]
        public void ImportCompletedBindChildIndirectlyThroughParentContainerBind()
        {
            var cat = CatalogFactory.CreateDefaultAttributed();
            var parent = new CompositionContainer(cat);

            CompositionBatch parentBatch = new CompositionBatch();
            CompositionBatch childBatch = new CompositionBatch();

            parentBatch.AddExportedObject<ICompositionService>(parent);
            parent.Compose(parentBatch);
            var child = new CompositionContainer(parent);

            var parentImporter = new MyNotifyImportImporter(parent);
            var childImporter = new MyNotifyImportImporter(child);

            parentBatch = new CompositionBatch();
            parentBatch.AddPart(parentImporter);
            childBatch.AddParts(childImporter, new MyNotifyImportExporter());

            parent.Compose(parentBatch);
            child.Compose(childBatch);

            Assert.AreEqual(1, parentImporter.ImportCompletedCallCount);
            Assert.AreEqual(1, childImporter.ImportCompletedCallCount);

            MyNotifyImportExporter parentExporter = parent.GetExportedObject<MyNotifyImportExporter>("MyNotifyImportExporter");
            Assert.AreEqual(1, parentExporter.ImportCompletedCallCount);

            MyNotifyImportExporter childExporter = child.GetExportedObject<MyNotifyImportExporter>("MyNotifyImportExporter");
            Assert.AreEqual(1, childExporter.ImportCompletedCallCount);
        }

        [TestMethod]
        public void ImportCompletedGetExportedObjectLazy()
        {
            var cat = CatalogFactory.CreateDefaultAttributed();
            CompositionContainer container = new CompositionContainer(cat);

            NotifyImportExportee.InstanceCount = 0;
            NotifyImportExportsLazy notifyee = container.GetExportedObject<NotifyImportExportsLazy>("NotifyImportExportsLazy");
            Assert.IsNotNull(notifyee, "Expecting bound type");
            Assert.IsNotNull(notifyee.Imports, "Expecting Imports to be populated");
            Assert.IsTrue(notifyee.NeedRefresh, "Expecting import to put class in pending state");
            Assert.AreEqual(3, notifyee.Imports.Count, "Expecting 3 Exports before filtering");
            Assert.AreEqual(0, NotifyImportExportee.InstanceCount, "Not instance expected before pull");
            Assert.AreEqual(0, notifyee.realImports.Count, "Expecting collection to be empty before pull");
            Assert.AreEqual(2, notifyee.RealImports.Count, "Expecting 2 real values after pull");
            Assert.AreEqual(1, notifyee.RealImports[0].Id, "Expecting distinct activated instance");
            Assert.AreEqual(3, notifyee.RealImports[1].Id, "Expecting distinct  activated instance");
            Assert.AreEqual(2, NotifyImportExportee.InstanceCount, "2 instances expected after pull");
        }

        [TestMethod]
        public void ImportCompletedGetExportedObjectEager()
        {
            var cat = CatalogFactory.CreateDefaultAttributed();
            CompositionContainer container = new CompositionContainer(cat);

            NotifyImportExportee.InstanceCount = 0;
            var notifyee = container.GetExportedObject<NotifyImportExportsEager>("NotifyImportExportsEager");
            Assert.IsNotNull(notifyee, "Expecting bound type");
            Assert.IsNotNull(notifyee.Imports, "Expecting Imports to be populated");
            Assert.AreEqual(3, notifyee.Imports.Count, "Expecting 3 Exports before filtering");
            Assert.AreEqual(2, NotifyImportExportee.InstanceCount, "Expecting concrete instances already pulled");
            Assert.AreEqual(2, notifyee.realImports.Count, "Expecting collection to be populated");
            Assert.AreEqual(2, notifyee.RealImports.Count, "Expecting 2 real values after import");
            Assert.AreEqual(1, notifyee.RealImports[0].Id, "Expecting distinct activated instance");
            Assert.AreEqual(3, notifyee.RealImports[1].Id, "Expecting distinct activated instance");
            Assert.AreEqual(2, NotifyImportExportee.InstanceCount, "Expecting no more instances after read");
        }
    }

    public class NotifyImportExportee
    {
        public NotifyImportExportee(int id)
        {
            Id = id;
            InstanceCount++;
        }

        public int Id { get; set; }

        public static int InstanceCount { get; set; }
    }

    public class NotifyImportExporter
    {

        public NotifyImportExporter()
        {
        }

        [Export()]
        [ExportMetadata("Filter", false)]
        public NotifyImportExportee Export1
        {
            get
            {
                return new NotifyImportExportee(1);
            }
        }

        [Export()]
        [ExportMetadata("Filter", true)]
        public NotifyImportExportee Export2
        {
            get
            {
                return new NotifyImportExportee(2);
            }
        }

        [Export()]
        [ExportMetadata("Filter", false)]
        public NotifyImportExportee Export3
        {
            get
            {
                return new NotifyImportExportee(3);
            }
        }

    }

    [Export("NotifyImportExportsLazy")]
    public class NotifyImportExportsLazy : IPartImportsSatisfiedNotification
    {
        public NotifyImportExportsLazy()
        {
            NeedRefresh = false;
        }

        [ImportMany(typeof(NotifyImportExportee))]
        public ExportCollection<NotifyImportExportee> Imports { get; set; }

        public bool NeedRefresh { get; set; }

        public void OnImportsSatisfied()
        {
            NeedRefresh = true;
        }

        internal Collection<NotifyImportExportee> realImports = new Collection<NotifyImportExportee>();

        public Collection<NotifyImportExportee> RealImports
        {
            get 
            {
                if (NeedRefresh)
                {
                    realImports.Clear();
                    foreach (var import in Imports)
                    {
                        if (!((bool)import.Metadata["Filter"]))
                        {
                            realImports.Add(import.GetExportedObject());
                        }
                    }
                    NeedRefresh = false;
                }
                return realImports;
            }
        }
    }

    [Export("NotifyImportExportsEager")]
    public class NotifyImportExportsEager : IPartImportsSatisfiedNotification
    {
        public NotifyImportExportsEager()
        {
        }

        [ImportMany]
        public ExportCollection<NotifyImportExportee> Imports { get; set; }

        public void OnImportsSatisfied()
        {
            realImports.Clear();
            foreach (var import in Imports)
            {
                if (!((bool)import.Metadata["Filter"]))
                {
                    realImports.Add(import.GetExportedObject());
                }
            }
        }

        internal Collection<NotifyImportExportee> realImports = new Collection<NotifyImportExportee>();

        public Collection<NotifyImportExportee> RealImports
        {
            get
            {
                return realImports;
            }
        }
    }
   
    [Export("MyNotifyImportExporter")]
    public class MyNotifyImportExporter : IPartImportsSatisfiedNotification
    {
        [Import]
        public ICompositionService ImportSomethingSoIGetImportCompletedCalled { get; set; }

        public int ImportCompletedCallCount { get; set; }
        public void OnImportsSatisfied()
        {
            ImportCompletedCallCount++;
        }
    }

    public class MyNotifyImportImporter : IPartImportsSatisfiedNotification
    {
        private CompositionContainer container;
        public MyNotifyImportImporter(CompositionContainer container)
        {
            this.container = container;
        }
        [Import("MyNotifyImportExporter")]
        public MyNotifyImportExporter MyNotifyImportExporter { get; set; }

        public int ImportCompletedCallCount { get; set; }
        public void OnImportsSatisfied()
        {
            ImportCompletedCallCount++;
        }
    }

    [Export("LowerCaseString")]
    public class LowerCaseString
    {
        public string String { get; private set; }
        public LowerCaseString(string s)
        {
            String = s.ToLower();
        }
    }

    public class UpperCaseStringComponent : IPartImportsSatisfiedNotification
    {
        public UpperCaseStringComponent()
        {
            UpperCaseStrings = new List<string>();
        }
        ExportCollection<LowerCaseString> lowerCaseString = new ExportCollection<LowerCaseString>();
        
        [ImportMany("LowerCaseString", AllowRecomposition = true)]
        public ExportCollection<LowerCaseString> LowerCaseStrings { 
            get { return lowerCaseString; }
            set { lowerCaseString = value; }
        }

        public List<string> UpperCaseStrings { get; set; }

        public int ImportCompletedCallCount { get; set; }

        // This method gets called whenever a bind is completed and any of 
        // of the imports have changed, but ar safe to use now.
        public void OnImportsSatisfied()
        {
            UpperCaseStrings.Clear();
            foreach (var i in LowerCaseStrings)
                UpperCaseStrings.Add(i.GetExportedObject().String.ToUpper());

            ImportCompletedCallCount++;
        }
    }
}
