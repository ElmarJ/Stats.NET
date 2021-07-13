// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;

namespace System.ComponentModel.Composition
{
    /// <summary>
    ///     Classes or interfaces marked with <see cref="PartExportsInheritedAttribute"/>
    ///     will have all their exports propagated to every class that inherits from it. Thus essentially
    ///     having all its exports re-exported by every sub-class. This attribute should generally only be
    ///     used if the inheriters are not expected to define their own exports or imports.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public sealed class PartExportsInheritedAttribute : Attribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="PartExportsInheritedAttribute"/> class.
        /// </summary>
        public PartExportsInheritedAttribute()
        {
        } 
    }
}
