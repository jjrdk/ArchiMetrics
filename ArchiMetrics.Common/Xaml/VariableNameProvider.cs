// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VariableNameProvider.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the VariableNameProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Common.Xaml
{
	using System.Collections.Generic;

	internal class VariableNameProvider
	{
		private static readonly Dictionary<string, int> UnnamedCache = new Dictionary<string, int>();

		public static string Get(string className)
		{
			if (UnnamedCache.ContainsKey(className) == false)
			{
				UnnamedCache.Add(className, 0);
			}

			var name = className.ToLower() + UnnamedCache[className];

			UnnamedCache[className]++;

			return name.Replace(".", "_");
		}
	}
}
