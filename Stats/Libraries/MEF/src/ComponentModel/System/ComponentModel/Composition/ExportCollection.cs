// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition.Primitives;

namespace System.ComponentModel.Composition
{
    /// <summary>
    ///     Represents a collection of <see cref="Export"/> objects.
    /// </summary>
    public class ExportCollection : Collection<Export>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ExportCollection"/> class 
        ///     that is empty.
        /// </summary>
        public ExportCollection() :
            this((IEnumerable<Export>)null)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ExportCollection"/> class,
        ///     adding the specified <see cref="IEnumerable{T}"/> of <see cref="Export"/>
        ///     objects to the collection.
        /// </summary>
        /// <param name="items">
        ///     An <see cref="IEnumerable{T}"/> of <see cref="Export"/> objects to add 
        ///     to the <see cref="ExportCollection"/>; or <see langword="null"/> to create 
        ///     an <see cref="ExportCollection"/> that is empty.
        /// </param>
        public ExportCollection(IEnumerable<Export> items)
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
