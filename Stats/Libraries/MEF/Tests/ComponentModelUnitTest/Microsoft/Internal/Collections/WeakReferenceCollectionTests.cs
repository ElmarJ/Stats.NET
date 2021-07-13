// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Internal.Collections
{
    [TestClass]
    public class WeakReferenceCollectionTests
    {
        [TestMethod]
        public void Add_ObjectShouldGetCollected()
        {
            var obj = new object();
            var wrc = new WeakReferenceCollection<object>();

            wrc.Add(obj);

            var wr = new WeakReference(obj);
            obj = null;

            Assert.IsNotNull(wr.Target, "Object should NOT have been collected yet!");

            GC.Collect();
            GC.WaitForPendingFinalizers();

            Assert.IsNull(wr.Target, "Object should have been collected!");

            GC.KeepAlive(wrc);
        }

        [TestMethod]
        public void Remove_ObjectShouldGetRemoved()
        {
            var obj = new object();
            var wrc = new WeakReferenceCollection<object>();

            wrc.Add(obj);

            Assert.AreEqual(1, wrc.AliveItemsToArray().Length, "Should have 1 item!");
            
            wrc.Remove(obj);

            Assert.AreEqual(0, wrc.AliveItemsToArray().Length, "Should have 0 item!");
        }

        [TestMethod]
        public void AliveItemsToArray_ShouldReturnAllItems()
        {
            var list = new object[] {new object(), new object(), new object()};
            var wrc = new WeakReferenceCollection<object>();

            foreach (object obj in list)
            {
                wrc.Add(obj);
            }

            Assert.AreEqual(list.Length, wrc.AliveItemsToArray().Length, "Should have same number of items!");
        }

        [TestMethod]
        public void AliveItemsToArray_ShouldReturnAllAliveItems()
        {
            var list = new object[] { new object(), new object(), new object() };
            var wrc = new WeakReferenceCollection<object>();

            var obj1 = new object();
            wrc.Add(obj1);

            foreach (object obj in list)
            {
                wrc.Add(obj);
            }

            var obj2 = new object();
            wrc.Add(obj2);

            Assert.AreEqual(list.Length + 2, wrc.AliveItemsToArray().Length, "Should have same number of items!");

            obj1 = obj2 = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();

            var aliveItems = wrc.AliveItemsToArray();
            Assert.AreEqual(list.Length, aliveItems.Length, "Should have 2 less items!");

            Assert.AreEqual(list[0], aliveItems[0]);
            Assert.AreEqual(list[1], aliveItems[1]);
            Assert.AreEqual(list[2], aliveItems[2]);
        }

        [TestMethod]
        public void AliveItemsToArray_ShouldReturnEmpty()
        {
            var wrc = new WeakReferenceCollection<object>();
            Assert.AreEqual(0, wrc.AliveItemsToArray().Length, "Should have 0 items!");
        }
    }
}
