// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProjectMetricsRepository.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the IProjectMetricsRepository type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Common.Structure
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using ArchiMetrics.Common.Metrics;

	public interface IProjectMetricsRepository : IDisposable
	{
		Task<IEnumerable<IProjectMetric>> Get(string solutionPath);
	}
}