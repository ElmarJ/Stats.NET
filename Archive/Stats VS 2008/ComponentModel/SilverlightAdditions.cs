// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Diagnostics;

namespace System
{
    [Conditional("NOT_SILVERLIGHT")]    // Trick so that the attribute is never actually applied
    internal sealed class SerializableAttribute : Attribute
    {
    }
}
