// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.UnitTesting;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.Composition
{
    [TestClass]
    public class CompositionErrorIdTests
    {
        [TestMethod]
        public void CompositionErrorIdsAreInSyncWithErrorIds()
        {
            AssertAreInSync<ErrorId, CompositionErrorId>();
        }

        [TestMethod]
        public void ErrorIdsAreInSyncWithCompositionErrorIds()
        {
            AssertAreInSync<CompositionErrorId, ErrorId>();
        }

        private void AssertAreInSync<T1, T2>()
            where T1 : struct
            where T2 : struct
        {
            var values = TestServices.GetEnumValues<T1>();

            foreach (T1 value in values)
            {
                string name1 = Enum.GetName(typeof(T1), value);
                string name2 = Enum.GetName(typeof(T2), value);

                Assert.AreEqual(name1, name2, "{0} contains a value that {1} does not have. These enums need to be in sync.", typeof(T1), typeof(T2));
            }
        }
    }
}
