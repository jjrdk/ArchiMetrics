// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICodeMetricsCalculator.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ICodeMetricsCalculator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Common.Metrics
{
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using Microsoft.CodeAnalysis;

	public interface ICodeMetricsCalculator
	{
		Task<IEnumerable<INamespaceMetric>> Calculate(Project project, Solution solution);

		Task<IEnumerable<INamespaceMetric>> Calculate(Solution solution);
	}
}
