// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Utils.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the Utils type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis
{
	using System.Collections.Generic;
	using System.Linq;

	internal static class Utils
	{
		public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T> source) where T : class
		{
			return source.Where(x => x != null);
		}
	}
}