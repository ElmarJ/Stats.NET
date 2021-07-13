// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace Microsoft.Internal.Collections
{
    internal static partial class CollectionServices
    {
        public static Type GetEnumerableElementType(Type type)
        {
            if (type == typeof(string) || !typeof(IEnumerable).IsAssignableFrom(type))
            {
                return null;
            }

            Type closedType;
            if (ReflectionServices.TryGetGenericInterfaceType(type, typeof(IEnumerable<>), out closedType))
            {
                return closedType.GetGenericArguments()[0];
            }

            return null;
        }

        public static Type GetCollectionElementType(Type type)
        {
            Type closedType;
            if (ReflectionServices.TryGetGenericInterfaceType(type, typeof(ICollection<>), out closedType))
            {
                return closedType.GetGenericArguments()[0];
            }

            return null;
        }

        public static Collection<T> ToCollection<T>(this IEnumerable<T> source)
        {
            Assumes.NotNull(source);

            return new Collection<T>(source.ToArray());
        }

        public static ReadOnlyCollection<T> ToReadOnlyCollection<T>(this IEnumerable<T> source)
        {
            Assumes.NotNull(source);

            return new ReadOnlyCollection<T>(source.ToArray());
        }

        public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T> source) where T : class
        {
            Assumes.NotNull(source);

            return source.Where(element => element != null);
        }

        public static IEnumerable<TResult> ConcatMany<TSource, TResult>(this ICollection<TSource> source, Func<TSource, IEnumerable<TResult>> selector)
        {
            // this will evaluate the function upfront without eveulating each IEnumerable
            // also removing empty enumerables
            IList<IEnumerable<TResult>> evaluatedSource = source.Select(element => selector(element)).Where(element => element.Any()).ToList();
            return evaluatedSource.SelectMany(elements => elements);
        }

        public static IEnumerable<T> ConcatAllowingNull<T>(this IEnumerable<T> source, IEnumerable<T> second)
        {
            if (source == null)
            {
                return second;
            }

            if (second == null)
            {
                return source;
            }

            return source.Concat(second);
        }
 
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach(T t in source)
            {
                action.Invoke(t);
            }
        }

        public static EnumerableCardinality GetCardinality<T>(this IEnumerable<T> source)
        {
            Assumes.NotNull(source);

            // Cast to ICollection instead of ICollection<T> for performance reasons.
            ICollection collection = source as ICollection;
            if (collection != null)
            {
                switch (collection.Count)
                {
                    case 0:
                        return EnumerableCardinality.Zero;

                    case 1:
                        return EnumerableCardinality.One;

                    default:
                        return EnumerableCardinality.TwoOrMore;
                }
            }

            using (var enumerator = source.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                {
                    return EnumerableCardinality.Zero;
                }

                if (!enumerator.MoveNext())
                {
                    return EnumerableCardinality.One;
                }

                return EnumerableCardinality.TwoOrMore;
            }
        }

        public static bool FastAny<T>(this IEnumerable<T> source)
        {
            // Enumerable.Any<T> underneath doesn't cast to ICollection, 
            // like it does with many of the other LINQ methods.
            // Below is significantly (4x) when mainly working with ICollection
            // sources and a little slower if working with mainly IEnumerable<T> 
            // sources.

            // Cast to ICollection instead of ICollection<T> for performance reasons.
            ICollection collection = source as ICollection;
            if (collection != null)
            {
                return collection.Count > 0;
            }

            return source.Any();
        }

        public static Stack<T> Copy<T>(this Stack<T> stack)
        {
            Assumes.NotNull(stack);

            // Stack<T>.GetEnumerator walks from top to bottom 
            // of the stack, whereas Stack<T>(IEnumerable<T>) 
            // pushes to bottom from top, so we need to reverse 
            // the stack to get them in the right order.
            return new Stack<T>(stack.Reverse());
        }
    }
}
