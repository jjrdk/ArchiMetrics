// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MetricOverview.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the MetricOverview type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Common.Metrics
{
	using System.Collections.Generic;

	public class MetricOverview
	{
		public MetricOverview(int sourceLinesOfCode, IEnumerable<ITypeMetric> metrics)
		{
			SourceLinesOfCode = sourceLinesOfCode;
			Metrics = metrics;
		}

		public int SourceLinesOfCode { get; set; }

		public IEnumerable<ITypeMetric> Metrics { get; set; }
	}
}
