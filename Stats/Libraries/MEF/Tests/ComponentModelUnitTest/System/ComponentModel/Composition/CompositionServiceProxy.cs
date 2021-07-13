// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;

namespace System.ComponentModel.Composition
{
    internal class CompositionServiceProxy : ICompositionService
    {
        private readonly CompositionContainer _container;

        public CompositionServiceProxy(CompositionContainer container)
        {
            this._container = container;
        }

        public void SatisfyImports(ComposablePart part, bool registerForRecomposition)
        {
            this._container.SatisfyImports(part, registerForRecomposition);
        }

        public void UnregisterForRecomposition(ComposablePart part)
        {
            this._container.UnregisterForRecomposition(part);
        }
    }
}
