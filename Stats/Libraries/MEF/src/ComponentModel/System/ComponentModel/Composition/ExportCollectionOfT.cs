// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace System.ComponentModel.Composition
{
    /// <summary>
    ///     Represents a collection of <see cref="Export{T}"/> objects.
    /// </summary>
    public class ExportCollection<T> : Collection<Export<T>>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ExportCollection{T}"/> class 
        ///     that is empty.
        /// </summary>
        public ExportCollection() :
            this((IEnumerable<Export<T>>)null)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ExportCollection{T}"/> class,
        ///     adding the specified <see cref="IEnumerable{T}"/> of <see cref="Export{T}"/> 
        ///     objects to the collection.
        /// </summary>
        /// <param name="items">
        ///     An <see cref="IEnumerable{T}"/> of <see cref="Export{T}"/> objects to add 
        ///     to the <see cref="ExportCollection{T}"/>; or <see langword="null"/> to create an 
        ///     <see cref="ExportCollection{T}"/> that is empty.
        /// </param>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public ExportCollection(IEnumerable<Export<T>> items)
        {
            if (items != null)
            {
                foreach (var item in items)
                {
                    this.Add(item);
                }
            }
        }
    }
}
