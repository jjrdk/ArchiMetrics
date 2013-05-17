// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProjectMetricsCalculator.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the IProjectMetricsCalculator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMeter.Common.Metrics
{
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using Roslyn.Services;

	public interface IProjectMetricsCalculator
	{
		bool IgnoreGeneratedCode { get; set; }

		Task<IEnumerable<NamespaceMetric>> Calculate(IProject project);
	}
}