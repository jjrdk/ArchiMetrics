// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransformerBase.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the TransformerBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Model
{
	using System.Collections.Concurrent;
	using System.Text.RegularExpressions;

	internal abstract class TransformerBase
	{
		private readonly ConcurrentDictionary<string, Regex> _regexes = new ConcurrentDictionary<string, Regex>();

		protected Regex GetTransform(string pattern)
		{
			return _regexes.GetOrAdd(pattern, x => new Regex(x, RegexOptions.Compiled));
		}
	}
}