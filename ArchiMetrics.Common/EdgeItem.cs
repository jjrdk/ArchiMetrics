// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EdgeItem.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the EdgeItem type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Common
{
	using System;
	using System.Collections.Generic;

	[Serializable]
	public class EdgeItem
	{
		public string Dependant { get; set; }

		public string Dependency { get; set; }

		public IEnumerable<EvaluationResult> CodeIssues { get; set; }

		public int MergedEdges { get; set; }

		public int DependantLinesOfCode { get; set; }

		public int DependantComplexity { get; set; }

		public double DependantMaintainabilityIndex { get; set; }

		public int DependencyLinesOfCode { get; set; }

		public int DependencyComplexity { get; set; }

		public double DependencyMaintainabilityIndex { get; set; }

		public override string ToString()
		{
			return string.Format("({0} -> {1})", Dependant, Dependency);
		}
	}
}
