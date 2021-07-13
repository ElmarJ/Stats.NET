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
    ///     Represents a collection of <see cref="Export{T, TMetadataView}"/> objects.
    /// </summary>
    public class ExportCollection<T, TMetadataView> : Collection<Export<T, TMetadataView>>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ExportCollection{T, TMetadataView}"/> class 
        ///     that is empty.
        /// </summary>
        public ExportCollection() :
            this((IEnumerable<Export<T, TMetadataView>>)null)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ExportCollection{T, TMetadataView}"/> class, 
        ///     adding the specified <see cref="IEnumerable{T}"/> of <see cref="Export{T, TMetadataView}"/> 
        ///     objects to the collection.
        /// </summary>
        /// <param name="items">
        ///     An <see cref="IEnumerable{T}"/> of <see cref="Export{T, TMetadataView}"/> objects to add 
        ///     to the <see cref="ExportCollection{T, TMetadataView}"/>; or <see langword="null"/> to create 
        ///     an <see cref="ExportCollection{T, TMetadataView}"/> that is empty.
        /// </param>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public ExportCollection(IEnumerable<Export<T, TMetadataView>> items)
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
