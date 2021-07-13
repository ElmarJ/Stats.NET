// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Factories;
using System.ComponentModel.Composition.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.Composition.AttributedModel;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.ReflectionModel;

namespace System.ComponentModel.Composition
{
    [TestClass]
    public class NotifyPropertyChangeTests
    {
        [TestMethod]
        public void BasicChangeTest()
        {
            var container = ContainerFactory.Create();
            CompositionBatch batch = new CompositionBatch();

            ImporterOfExporterNotifyPropertyChanged importer;
            ExporterNotifyPropertyChanged exporter;

            batch.AddParts(
                importer = new ImporterOfExporterNotifyPropertyChanged(), 
                new NotifyPropertyChangedComposablePart(exporter = new ExporterNotifyPropertyChanged(), container));
            container.Compose(batch);

            Assert.AreEqual(42, importer.Value);
            exporter.Value = 19;
            Assert.AreEqual(19, importer.Value, "Should have re-imported the property automatically");

            // Verify that doing it multiple times doesn't effect anything
            exporter.Value = 350;
            Assert.AreEqual(350, importer.Value, "Should have re-imported the property automatically");
        }

        [TestMethod]
        public void SingleValueChangedTest()
        {
            var container = ContainerFactory.Create();
            CompositionBatch batch = new CompositionBatch();
            ImporterOfExporterNotifyPropertyChanged importer;
            ExporterNotifyPropertyChanged exporter;

            batch.AddParts(
                importer = new ImporterOfExporterNotifyPropertyChanged(),
                new NotifyPropertyChangedComposablePart(exporter = new ExporterNotifyPropertyChanged(), container));
            container.Compose(batch);

            exporter.FirePropertyChange("foo");
            Assert.AreEqual(42, importer.Value, "Should NOT have changed after random prop change notification");
            Assert.AreEqual(2, importer.SecondValue, "Should NOT have changed after random prop change notification");
        }

        [TestMethod]
        public void ChangeAfterContainerDisposeTest()
        {
            var container = ContainerFactory.Create();
            CompositionBatch batch = new CompositionBatch();
            ImporterOfExporterNotifyPropertyChanged importer;
            ExporterNotifyPropertyChanged exporter;

            batch.AddParts(
                importer = new ImporterOfExporterNotifyPropertyChanged(),
                new NotifyPropertyChangedComposablePart(exporter = new ExporterNotifyPropertyChanged(), container));
            container.Compose(batch);

            exporter.Value = 209;
            Assert.AreEqual(209, importer.Value, "Should have re-imported value on property change");

            container.Dispose();
            exporter.Value = 409;
            Assert.AreEqual(209, importer.Value, "Should NOT have re-imported value on property change after the container was disposed");
        }
    }

    internal class NotifyPropertyChangedComposablePart : ReflectionComposablePart
    {
        private INotifyPropertyChanged _componentInstance;
        private CompositionContainer _container;

        public NotifyPropertyChangedComposablePart(INotifyPropertyChanged componentInstance, CompositionContainer container)
            : base(AttributedModelDiscovery.CreatePartDefinition(componentInstance.GetType(), null, true, (ICompositionElement)null), componentInstance)
        {
            _componentInstance = componentInstance;
            _container = container;
            _componentInstance.PropertyChanged += new PropertyChangedEventHandler(componentInstance_PropertyChanged);
        }

        public void componentInstance_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                // Fake out a recompose by removing and adding ourselves to the container.
                CompositionBatch batch = new CompositionBatch();
                batch.RemovePart(this);
                batch.AddPart(this);
                _container.Compose(batch);
            }
            catch (ObjectDisposedException)
            {
                _container = null;
                _componentInstance.PropertyChanged -= new PropertyChangedEventHandler(componentInstance_PropertyChanged);
            }
        }
    }
}
