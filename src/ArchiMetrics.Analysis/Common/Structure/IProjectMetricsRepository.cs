// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProjectMetricsRepository.cs" company="Reimers.dk">
//   Copyright © Matthias Friedrich, Reimers.dk 2014
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the IProjectMetricsRepository type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Common.Structure
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Metrics;

    public interface IProjectMetricsRepository : IDisposable
	{
		Task<IEnumerable<IProjectMetric>> Get(string solutionPath);
	}
}