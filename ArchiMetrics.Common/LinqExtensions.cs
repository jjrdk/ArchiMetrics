// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LinqExtensions.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the LinqExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Common
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public static class LinqExtensions
	{
		public static IEnumerable<T> DistinctBy<T, TOut>(this IEnumerable<T> source, Func<T, TOut> func)
		{
			var comparer = new FuncComparer<T, TOut>(func);
			return source.Distinct(comparer);
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