// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProjectMetricsCalculator.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the IProjectMetricsCalculator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Common.Metrics
{
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using Microsoft.CodeAnalysis;

	public interface IProjectMetricsCalculator
	{
		Task<IEnumerable<IProjectMetric>> Calculate(Solution solution);
		
		Task<IProjectMetric> Calculate(Project project, Solution solution);
	}
}