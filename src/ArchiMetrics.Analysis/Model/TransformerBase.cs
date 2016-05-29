// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransformerBase.cs" company="Reimers.dk">
//   Copyright © Matthias Friedrich, Reimers.dk 2014
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
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