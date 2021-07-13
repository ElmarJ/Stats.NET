// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Internal
{
    [TestClass]
    public class WeakReferenceTests
    {
        [TestMethod]
        public void IsAlive_BeforeAfterCollection()
        {
            var obj = new object();
            var wr = new WeakReference<object>(obj);
            Assert.IsTrue(wr.IsAlive);

            obj = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();

            Assert.IsFalse(wr.IsAlive);
        }

        [TestMethod]
        public void Target_BeforeAfterCollection()
        {
            var obj = new object();
            var wr = new WeakReference<object>(obj);
            Assert.IsNotNull(wr.Target);

            obj = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();

            Assert.IsNull(wr.Target);            
        }

        [TestMethod]
        public void GetHashCode_SameAsGivenObject()
        {
            var obj = new Object();
            var wr = new WeakReference<object>(obj);

            var hashCode = obj.GetHashCode();
            
            Assert.AreEqual(hashCode, wr.GetHashCode());
            
            obj = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();

            Assert.IsNull(wr.Target);
            Assert.AreEqual(hashCode, wr.GetHashCode());
        }

        [TestMethod]
        public void Equals_SameReference_ShouldBeEqual()
        {
            var obj = new object();
            var wr = new WeakReference<object>(obj);

            Assert.IsTrue(wr.Equals(wr));

            obj = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();

            Assert.IsTrue(wr.Equals(wr));
        }

        [TestMethod]
        public void Equals_DifferentReference_ShouldBeEqual()
        {
            var obj = new object();
            var wr = new WeakReference<object>(obj);
            var wr2 = new WeakReference<object>(obj);

            Assert.IsTrue(wr.Equals(wr2));
            Assert.IsTrue(wr2.Equals(wr));
        }

        [TestMethod]
        public void Equals_DifferentReferenceAfterCollection_ShouldNotBeEqual()
        {
            var obj = new object();
            var wr = new WeakReference<object>(obj);
            var wr2 = new WeakReference<object>(obj);

            obj = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();

            Assert.IsFalse(wr.Equals(wr2));
            Assert.IsFalse(wr2.Equals(wr));
        }

        [TestMethod]
        public void Equals_NonWRObject_ShouldNotBeEqual()
        {
            var obj = new object();
            var wr = new WeakReference<object>(obj);

            Assert.IsFalse(wr.Equals(obj));
        }
    }
}
