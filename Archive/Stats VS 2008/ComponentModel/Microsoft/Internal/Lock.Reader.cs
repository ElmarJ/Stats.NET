// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Microsoft.Internal
{
    internal class ReadLock : IDisposable
    {
        private readonly Lock _lock;
        private int _isDisposed = 0;

        public ReadLock(Lock @lock)
        {
            this._lock = @lock;
            this._lock.EnterReadLock();
        }

        ~ReadLock()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (Interlocked.CompareExchange(ref this._isDisposed, 1, 0) == 0)
            {
                this._lock.ExitReadLock();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
