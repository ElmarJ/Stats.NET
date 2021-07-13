// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
#if !CLR40
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Internal;

namespace System.Threading
{
    public class LazyInit<T>
    {
        private T _value = default(T);
        private bool _isValueCreated = false;
        private Func<T> _valueSelector = null;

        public LazyInit(Func<T> valueSelector)
        {
            Requires.NotNull(valueSelector, "valueSelector");

            this._valueSelector = valueSelector;
        }

        public T Value
        {
            get
            {
                if (!this._isValueCreated)
                {
                    this._value = this._valueSelector.Invoke();
                    this._valueSelector = null;
                    this._isValueCreated = true;
                }
                return this._value;
            }
        }
    }
}
#endif
