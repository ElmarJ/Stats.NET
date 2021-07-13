// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Factories;
using System.Linq;
using System.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.Composition.AttributedModel;
using System.ComponentModel.Composition.ReflectionModel;
using System.ComponentModel.Composition.Primitives;

namespace System.ComponentModel.Composition
{
    [TestClass]
    public class CompositionServiceExtensionsTests
    {
        [TestMethod]
        public void SatisfyImports_BooleanOverride_NullAsCompositionService()
        {
            ICompositionService compositionService = null;
            ExceptionAssert.ThrowsArgument<ArgumentNullException>("compositionService", () =>
            {
                compositionService.SatisfyImports(PartFactory.Create());
            });
        }

        [TestMethod]
        public void SatisfyImports_BooleanOverride_PartAndFalseHaveBeenPassed()
        {
            MockCompositionService compositionService = new MockCompositionService();
            ComposablePart part = PartFactory.Create();

            bool importsSatisfiedCalled = false;
            compositionService.ImportsSatisfied += delegate(object sender, SatisfyImportsEventArgs e)
            {
                Assert.IsFalse(importsSatisfiedCalled);
                Assert.AreEqual(part, e.Part);
                Assert.IsFalse(e.RegisterForRecomposition);
                Assert.IsFalse(compositionService.RegisteredParts.Contains(e.Part));
                importsSatisfiedCalled = true;
            };

            compositionService.SatisfyImports(part);
            Assert.IsTrue(importsSatisfiedCalled);
        }


        [TestMethod]
        public void SatisfyImports_AttributedOverride_NullAsCompositionService()
        {
            ICompositionService compositionService = null;
            ExceptionAssert.ThrowsArgument<ArgumentNullException>("compositionService", () =>
            {
                compositionService.SatisfyImports(new MockAttributedPart());
            });
        }

        [TestMethod]
        public void SatisfyImports_AttributedOverride_NullAsAttributedPart()
        {
            MockCompositionService compositionService = new MockCompositionService();
            ExceptionAssert.ThrowsArgument<ArgumentNullException>("attributedPart", () =>
            {
                compositionService.SatisfyImports((object)null);
            });
        }

        [TestMethod]
        public void SatisfyImports_AttributedOverride_PartAndFalseHaveBeenPassed()
        {
            MockCompositionService compositionService = new MockCompositionService();
            object attributedPart = new MockAttributedPart();

            bool importsSatisfiedCalled = false;
            compositionService.ImportsSatisfied += delegate(object sender, SatisfyImportsEventArgs e)
            {
                Assert.IsFalse(importsSatisfiedCalled);
                Assert.IsTrue(e.Part is ReflectionComposablePart);
                Assert.IsTrue(((ReflectionComposablePart)e.Part).Definition.GetPartType() == typeof(MockAttributedPart));
                Assert.IsFalse(e.RegisterForRecomposition);
                Assert.IsFalse(compositionService.RegisteredParts.Contains(e.Part));
                importsSatisfiedCalled = true;
            };

            compositionService.SatisfyImports(attributedPart);
            Assert.IsTrue(importsSatisfiedCalled);
        }

        [TestMethod]
        public void SatisfyImports_AttributedAndBooleanOverride_NullAsCompositionService()
        {
            ICompositionService compositionService = null;
            ExceptionAssert.ThrowsArgument<ArgumentNullException>("compositionService", () =>
            {
                compositionService.SatisfyImports(new MockAttributedPart(), true);
            });
        }

        [TestMethod]
        public void SatisfyImports_AttributedAndBooleanOverride_NullAsAttributedPart()
        {
            MockCompositionService compositionService = new MockCompositionService();
            ExceptionAssert.ThrowsArgument<ArgumentNullException>("attributedPart", () =>
            {
                compositionService.SatisfyImports((object)null, true);
            });
        }

        [TestMethod]
        public void SatisfyImports_AttributedAndBooleanOverride_PartAndFalseHaveBeenPassed()
        {
            MockCompositionService compositionService = new MockCompositionService();
            object attributedPart = new MockAttributedPart();

            bool importsSatisfiedCalled = false;
            compositionService.ImportsSatisfied += delegate(object sender, SatisfyImportsEventArgs e)
            {
                Assert.IsFalse(importsSatisfiedCalled);
                Assert.IsTrue(e.Part is ReflectionComposablePart);
                Assert.IsTrue(((ReflectionComposablePart)e.Part).Definition.GetPartType() == typeof(MockAttributedPart));
                Assert.IsTrue(e.RegisterForRecomposition);
                Assert.IsTrue(compositionService.RegisteredParts.Contains(e.Part));
                importsSatisfiedCalled = true;
            };

            compositionService.SatisfyImports(attributedPart, true);
            Assert.IsTrue(importsSatisfiedCalled);
        }



        internal class SatisfyImportsEventArgs : EventArgs
        {
            public SatisfyImportsEventArgs(ComposablePart part, bool registerForRecomposition)
            {
                this.Part = part;
                this.RegisterForRecomposition = registerForRecomposition;
            }

            public ComposablePart Part { get; private set; }
            public bool RegisterForRecomposition { get; private set; }
        }

        internal class MockCompositionService : ICompositionService
        {
            public ICollection<ComposablePart> RegisteredParts { get; private set; }

            public MockCompositionService()
            {
                this.RegisteredParts = new List<ComposablePart>();
            }

            public void SatisfyImports(ComposablePart part, bool registerForRecomposition)
            {
                if (registerForRecomposition)
                {
                    this.RegisteredParts.Add(part);
                }

                if (this.ImportsSatisfied != null)
                {
                    this.ImportsSatisfied.Invoke(this, new SatisfyImportsEventArgs(part, registerForRecomposition));
                }
            }

            public void UnregisterForRecomposition(ComposablePart part)
            {
                this.RegisteredParts.Remove(part);
            }


            public event EventHandler<SatisfyImportsEventArgs> ImportsSatisfied;
        }

        public class MockAttributedPart
        {
            public MockAttributedPart()
            {
            }
        }
    }
}
