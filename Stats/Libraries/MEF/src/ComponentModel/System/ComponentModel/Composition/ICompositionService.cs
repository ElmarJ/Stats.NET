// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;

namespace System.ComponentModel.Composition
{
    /// <summary>
    ///     Provides methods for composing <see cref="ComposablePart"/> objects.
    /// </summary>
    public interface ICompositionService
    {
        /// <summary>
        ///     Sets the imports of the specified composable part and registering for it for 
        ///     recomposition.
        /// </summary>
        /// <param name="part">
        ///     The <see cref="ComposablePart"/> to set the imports and register for recomposition.
        /// </param>
        /// <param name="registerForRecomposition">
        ///     Indicates whether the part will keep recomposing after the initial import satisfaction
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="part"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="CompositionException">
        ///     An error occurred during composition. <see cref="CompositionException.Errors"/> will
        ///     contain a collection of errors that occurred.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        ///     The <see cref="ICompositionService"/> has been disposed of.
        /// </exception>
        void SatisfyImports(ComposablePart part, bool registerForRecomposition);

        /// <summary>
        ///     Unregisters the specified part from recomposition.
        /// </summary>
        /// <param name="part">
        ///     The <see cref="ComposablePart"/> to unregister from recomposition.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="part"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        ///     The <see cref="ICompositionService"/> has been disposed of.
        /// </exception>
        void UnregisterForRecomposition(ComposablePart part);
    }
}