// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.UnitTesting
{
    public static class ExtendedAssert
    {
        /// <summary>
        ///     Verifies that the two specified objects are an instance of the same type.
        /// </summary>
        public static void IsInstanceOfSameType(object expected, object actual)
        {
            if (expected == null || actual == null)
            {
                Assert.AreSame(expected, actual);
                return;
            }

            Assert.AreSame(expected.GetType(), actual.GetType());
        }

        public static void ContainsLines(string value, params string[] lines)
        {
            StringReader reader = new StringReader(value);

            int count = 0;
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (count == lines.Length)
                {
                    Assert.Fail();
                }

                StringAssert.Contains(line, lines[count]);

                count++;
            }

            Assert.AreEqual(lines.Length, count, "Expectation: {0}; Result: {1}", String.Join(Environment.NewLine, lines), value);
        }
    }
}
