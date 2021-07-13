// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Internal;

namespace System.ComponentModel.Composition
{
    /// <summary>
    ///     Provides extension methods for the <see cref="ICompositionService"/> interface.
    /// </summary>
    public static partial class CompositionServiceExtensions
    {
        /// <summary>
        ///     Satisfies the imports of the specified composable part.
        /// </summary>
        /// <param name="part">
        ///     The <see cref="ComposablePart"/> to set the imports and register for recomposition.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="part"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="CompositionException">
        ///     An error occurred during composition. <see cref="CompositionException.Errors"/> will
        ///     contain a collection of errors that occurred.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        ///     The <see cref="CompositionContainer"/> has been disposed of.
        /// </exception>
        public static void SatisfyImports(this ICompositionService compositionService, ComposablePart part)
        {
            Requires.NotNull(compositionService, "compositionService");
            compositionService.SatisfyImports(part, false);
        }

        /// <summary>
        ///     Satisfies the imports of the specified attributed composable part and registers it for 
        ///     recomposition if requested.
        /// </summary>
        /// <param name="part">
        ///     The <see cref="ComposablePart"/> to set the imports and register for recomposition.
        /// </param>
        /// <param name="registerForRecomposition">
        ///     Indicates whether the part will keep recomposing after the initial import satisfaction
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="attributedPart"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="CompositionException">
        ///     An error occurred during composition. <see cref="CompositionException.Errors"/> will
        ///     contain a collection of errors that occurred.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        ///     The <see cref="ICompositionService"/> has been disposed of.
        /// </exception>
        public static ComposablePart SatisfyImports(this ICompositionService compositionService, object attributedPart, bool registerForRecomposition)
        {
            Requires.NotNull(compositionService, "compositionService");
            Requires.NotNull(attributedPart, "attributedPart");

            ComposablePart part = AttributedModelServices.CreatePart(attributedPart);
            compositionService.SatisfyImports(part, registerForRecomposition);

            return part;
        }

        /// <summary>
        ///     Satisfies the imports of the specified attributed composable part.
        /// </summary>
        /// <param name="part">
        ///     The <see cref="ComposablePart"/> to set the imports and register for recomposition.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="attributedPart"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="CompositionException">
        ///     An error occurred during composition. <see cref="CompositionException.Errors"/> will
        ///     contain a collection of errors that occurred.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        ///     The <see cref="ICompositionService"/> has been disposed of.
        /// </exception>
        public static void SatisfyImports(this ICompositionService compositionService, object attributedPart)
        {
            compositionService.SatisfyImports(attributedPart, false);
        }
    }
}