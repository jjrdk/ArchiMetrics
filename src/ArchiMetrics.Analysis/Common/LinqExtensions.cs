// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LinqExtensions.cs" company="Reimers.dk">
//   Copyright © Matthias Friedrich, Reimers.dk 2014
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the LinqExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Common
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    public static class LinqExtensions
    {
        public static T[] AsArray<T>(this IEnumerable<T> items)
        {
            var array = items as T[];
            return array ?? items.ToArray();
        }

        public static IEnumerable<T> DistinctBy<T, TOut>(this IEnumerable<T> source, Func<T, TOut> func)
        {
            var comparer = new FuncComparer<T, TOut>(func);
            return source.Distinct(comparer);
        }

        public static IEnumerable<T> WhereNot<T>(this IEnumerable<T> source, Func<T, bool> filter)
        {
            return source.Where(x => !filter(x));
        }

        public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T> source) where T : class
        {
            return source.Where(x => x != null);
        }

        public static IEnumerable<string> WhereNotNullOrWhitespace(this IEnumerable<string> source)
        {
            return source.Where(x => !string.IsNullOrWhiteSpace(x));
        }

        public static Collection<T> ToCollection<T>(this IEnumerable<T> source)
        {
            return new Collection<T>(source.AsArray());
        }

        public static bool In<T>(this T item, IEnumerable<T> collection)
        {
            return collection.Contains(item);
        }

        private class FuncComparer<T, TOut> : IEqualityComparer<T>
        {
            private readonly Func<T, TOut> _func;

            public FuncComparer(Func<T, TOut> func)
            {
                _func = func;
            }

            public bool Equals(T x, T y)
            {
                return _func(x).Equals(_func(y));
            }

            public int GetHashCode(T obj)
            {
                return _func(obj).GetHashCode();
            }
        }
    }
}