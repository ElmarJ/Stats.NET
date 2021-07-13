// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.UnitTesting
{
    public enum RetryMode : int
    {
        DoNotRetry = 0,
        Retry = 1,
    }
}
