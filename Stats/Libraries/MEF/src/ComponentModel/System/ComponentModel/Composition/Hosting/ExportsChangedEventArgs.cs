// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Internal;

namespace System.ComponentModel.Composition.Hosting
{
    /// <summary>
    ///     Provides data for the <see cref="ExportProvider.ExportsChanged"/> event.
    /// </summary>
    public class ExportsChangedEventArgs : EventArgs
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ExportsChangedEventArgs"/> class with 
        ///     the specified changed contract names.
        /// </summary>
        /// <param name="changedContractNames">
        ///     An <see cref="IEnumerable{T}"/> of strings representing the contract names of the 
        ///     exports that have changed in the <see cref="CompositionContainer"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="changedContractNames"/> is <see langword="null"/>.
        /// </exception>
        public ExportsChangedEventArgs(IEnumerable<string> changedContractNames)
        {
            Requires.NotNull(changedContractNames, "changedContractNames");

            this.ChangedContractNames = new ReadOnlyCollection<string>(changedContractNames.ToList());
        }

        /// <summary>
        ///     Gets the contract names of the exports that have changed.
        /// </summary>
        /// <value>
        ///     A <see cref="ReadOnlyCollection{T}"/> of strings representing the contract names of 
        ///     the exports that have changed in the <see cref="CompositionContainer"/>.
        /// </value>
        public ReadOnlyCollection<string> ChangedContractNames { get; private set; }
    }
}