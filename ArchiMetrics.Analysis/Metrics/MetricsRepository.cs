// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MetricsRepository.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the MetricsRepository type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Metrics
{
	using System;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using ArchiMetrics.Common;
	using ArchiMetrics.Common.Metrics;
	using ArchiMetrics.Common.Structure;
	using Microsoft.CodeAnalysis;

	internal class MetricsRepository : IProjectMetricsRepository
	{
		private readonly ConcurrentDictionary<string, Task<IEnumerable<IProjectMetric>>> _metrics = new ConcurrentDictionary<string, Task<IEnumerable<IProjectMetric>>>();
		private readonly IProjectMetricsCalculator _metricsCalculator;
		private readonly IProvider<string, Task<Solution>> _solutionProvider;

		public MetricsRepository(IProjectMetricsCalculator metricsCalculator, IProvider<string, Task<Solution>> solutionProvider)
		{
			_metricsCalculator = metricsCalculator;
			_solutionProvider = solutionProvider;
		}

		~MetricsRepository()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public Task<IEnumerable<IProjectMetric>> Get(string solutionPath)
		{
			return _metrics.GetOrAdd(
				solutionPath, 
				async path =>
				{
					var solution = await _solutionProvider.Get(path);
					return await _metricsCalculator.Calculate(solution);
				});
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				_metrics.Clear();
			}
		}
	}
}