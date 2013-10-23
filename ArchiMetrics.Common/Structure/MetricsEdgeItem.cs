// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MetricsEdgeItem.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the MetricsEdgeItem type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Common.Structure
{
	using System;
	using System.Collections.Generic;
	using ArchiMetrics.Common.CodeReview;

    [Serializable]
	public class MetricsEdgeItem : EdgeItemBase
    {
        public IEnumerable<EvaluationResult> CodeIssues { get; set; }

        public int DependantLinesOfCode { get; set; }

		public int DependantComplexity { get; set; }

		public double DependantMaintainabilityIndex { get; set; }

		public int DependencyLinesOfCode { get; set; }

		public int DependencyComplexity { get; set; }

		public double DependencyMaintainabilityIndex { get; set; }
    }
}
