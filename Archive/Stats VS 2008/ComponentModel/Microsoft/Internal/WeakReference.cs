// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;

namespace Microsoft.Internal
{
    internal class WeakReference<T> where T : class
    {
        private readonly WeakReference _weakReference;
        private readonly int _hashCode;

        public WeakReference(T obj)
        {
            Assumes.NotNull(obj);
            this._hashCode = obj.GetHashCode();
            this._weakReference = new WeakReference(obj);
        }

        public T Target
        {
            get
            {
                return (T) this._weakReference.Target;
            }
        }

        public bool IsAlive
        {
            get
            {
                return this._weakReference.IsAlive;
            }
        }

        public override bool Equals(object obj)
        {
            WeakReference<T> weakRef = obj as WeakReference<T>;

            if (weakRef == null)
            {
                return false;
            }

            object target1 = this.Target;
            object target2 = weakRef.Target;

            if (target1 == null || target2 == null)
            {
                return base.Equals(weakRef);
            }

            return target1.Equals(target2);
        }

        public override int GetHashCode()
        {
            return this._hashCode;
        }
    }
}
