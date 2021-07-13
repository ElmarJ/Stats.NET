// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Factories;
using System.Linq;
using System.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.Composition
{
    [TestClass]
    public class ComposablePartTests
    {
        [TestMethod]
        public void Constructor1_ShouldSetMetadataPropertyToEmptyDictionary()
        {
            var part = PartFactory.Create();

            EnumerableAssert.IsEmpty(part.Metadata);
        }

        [TestMethod]
        public void Constructor1_ShouldSetMetadataPropertyToReadOnlyDictionary()
        {
            var part = PartFactory.Create();

            ExceptionAssert.Throws<NotSupportedException>(() =>
            {
                part.Metadata["Value"] = "Value";
            });
        }

        [TestMethod]
        public void Constructor1_ShouldNotRequireDisposal()
        {
            var part = PartFactory.Create();

            Assert.IsFalse(part.RequiresDisposal);
        }

        [TestMethod]
        public void Metadata_WhenDisposed_ShouldThrowObjectDisposed()
        {
            var part = PartFactory.Create();
            part.Dispose();

            ExceptionAssert.ThrowsDisposed(part, () =>
            {
                var metadata = part.Metadata;
            });
        }

        [TestMethod]
        public void OnComposed_DoesNotThrow()
        {
            var part = PartFactory.Create();
            part.OnComposed();
        }

        [TestMethod]
        public void OnComposed_WhenDisposed_ShouldThrowObjectDisposed()
        {
            var part = PartFactory.Create();
            part.Dispose();

            ExceptionAssert.ThrowsDisposed(part, () =>
            {
                part.OnComposed();
            });
        }

        [TestMethod]
        public void Dispose_CallsGCSuppressFinalize()
        {
            bool finalizerCalled = false;

            var part = PartFactory.CreateDisposable(disposing =>
            {
                if (!disposing)
                {
                    finalizerCalled = true;
                }

            });

            part.Dispose();

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            Assert.IsFalse(finalizerCalled);
        }

        [TestMethod]
        public void Dispose_CallsDisposeBoolWithTrue()
        {
            var part = PartFactory.CreateDisposable(disposing =>
            {
                Assert.IsTrue(disposing);
            });

            part.Dispose();
        }

        [TestMethod]
        public void Dispose_CallsDisposeBoolOnce()
        {
            int disposeCount = 0;

            var part = PartFactory.CreateDisposable(disposing => 
            {
                disposeCount++;
            });

            part.Dispose();

            Assert.AreEqual(1, disposeCount);
        }
    }
}