// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransformRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the TransformRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Common.Structure
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
