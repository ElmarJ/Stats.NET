// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using Microsoft.Internal;

namespace System.ComponentModel.Composition.Primitives
{
    /// <summary>
    ///     Provides data for the <see cref="INotifyComposablePartCatalogChanged.Changed"/> event.
    /// </summary>
    public class ComposablePartCatalogChangedEventArgs : EventArgs
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ComposablePartCatalogChangedEventArgs"/> 
        ///     class with the specified changed <see cref="ComposablePartDefinition"/> objects.
        /// </summary>
        /// <param name="changedDefinitions">
        ///     An <see cref="IEnumerable{T}"/> of <see cref="ComposablePartDefinition"/> objects that 
        ///     have changed in the <see cref="ComposablePartCatalog"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="changedParts"/> is <see langword="null"/>.
        /// </exception>
        public ComposablePartCatalogChangedEventArgs(IEnumerable<ComposablePartDefinition> changedDefinitions)
        {
            Requires.NotNull(changedDefinitions, "changedDefinitions");

            this.ChangedDefinitions = changedDefinitions;
        }

        /// <summary>
        ///     Gets the identifiers of the parts that have changed.
        /// </summary>
        /// <value>
        ///     An <see cref="IEnumerable{T}"/> of <see cref="ComposablePartDefinition"/> objects that 
        ///     have changed in the <see cref="ComposablePartCatalog"/>.
        /// </value>
        public IEnumerable<ComposablePartDefinition> ChangedDefinitions { get; private set; }
    }
}
