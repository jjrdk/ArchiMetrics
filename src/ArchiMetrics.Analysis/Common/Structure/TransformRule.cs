// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransformRule.cs" company="Reimers.dk">
//   Copyright © Matthias Friedrich, Reimers.dk 2014
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the TransformRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Common.Structure
{
	public class TransformRule
	{
		public TransformRule(string name, string pattern)
		{
			Name = name;
			Pattern = pattern;
		}

		public string Pattern { get; private set; }

		public string Name { get; private set; }
	}
}
