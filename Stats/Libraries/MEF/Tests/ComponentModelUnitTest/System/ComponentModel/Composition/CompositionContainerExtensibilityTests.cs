// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.ComponentModel.Composition.Factories;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.Composition
{
    [TestClass]
    public class CompositionContainerExtensibilityTests
    {
        [TestMethod]
        public void Dispose_DoesNotThrow()
        {
            var container = CreateCustomCompositionContainer();
            container.Dispose();
        }

        [TestMethod]
        public void DerivedCompositionContainer_CanExportItself()
        {
            var container = CreateCustomCompositionContainer();
            container.AddAndComposeExportedObject<CustomCompositionContainer>(container);

            Assert.AreSame(container, container.GetExportedObject<CustomCompositionContainer>());
        }

        [TestMethod]
        public void ICompositionService_CanBeExported()
        {
            var container = CreateCustomCompositionContainer();
            container.AddAndComposeExportedObject<ICompositionService>(container);

            Assert.AreSame(container, container.GetExportedObject<ICompositionService>());
        }

        [TestMethod]
        public void CompositionContainer_CanBeExported()
        {
            var container = CreateCustomCompositionContainer();
            container.AddAndComposeExportedObject<CompositionContainer>(container);

            Assert.AreSame(container, container.GetExportedObject<CompositionContainer>());
        }

        private CustomCompositionContainer CreateCustomCompositionContainer()
        {
            return new CustomCompositionContainer();
        }

        // Type needs to be public otherwise container.GetExportedObject<CustomCompositionContainer> 
        // fails on Silverlight because it cannot construct a Export<T,M> factory. 
        public class CustomCompositionContainer : CompositionContainer
        {
        }
    }
}
