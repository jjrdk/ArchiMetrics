// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProjectMetricsCalculator.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the IProjectMetricsCalculator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Common.Metrics
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;

    /// <summary>
	/// Defines the interface for a project metrics calculator.
	/// </summary>
	public interface IProjectMetricsCalculator
	{
		/// <summary>
		/// Calculates the metrics for the passed <see cref="Solution"/>.
		/// </summary>
		/// <param name="solution">The <see cref="Solution"/> to calculate metrics for.</param>
		/// <returns>A <see cref="Task{TResult}"/> providing the metrics as an <see cref="IEnumerable{IProjectMetric}"/>.</returns>
		Task<IEnumerable<IProjectMetric>> Calculate(Solution solution);
	}
}