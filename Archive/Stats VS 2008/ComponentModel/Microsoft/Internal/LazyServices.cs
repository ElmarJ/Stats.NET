﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Text;
using System.Globalization;

namespace Microsoft.Internal
{
    internal static class LazyServices
    {
        public static LazyInit<T> AsLazy<T>(this T t)
            where T : class
        {
            return new LazyInit<T>(() => t);
        }

        public static T GetNotNullValue<T>(this LazyInit<T> lazy, string argument)
            where T : class
        {
            Assumes.NotNull(lazy);
            T value = lazy.Value;
            if (value == null)
            {
                throw new InvalidOperationException(
                    string.Format(CultureInfo.CurrentCulture, Strings.LazyServices_LazyResolvesToNull, typeof(T), argument));
            }

            return value;
        }
    }
}
