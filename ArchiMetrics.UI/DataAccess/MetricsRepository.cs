// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MetricsRepository.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the MetricsRepository type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.UI.DataAccess
{
	using System;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using ArchiMetrics.Analysis.Metrics;
	using ArchiMetrics.Common;
	using ArchiMetrics.Common.Metrics;
	using ArchiMetrics.Common.Structure;
	using Roslyn.Services;

	internal class MetricsRepository : IProjectMetricsRepository, IDisposable
	{
		private readonly ConcurrentDictionary<string, Task<IEnumerable<IProjectMetric>>> _metrics = new ConcurrentDictionary<string, Task<IEnumerable<IProjectMetric>>>();
		private readonly ICodeMetricsCalculator _metricsCalculator;
		private readonly IProvider<string, ISolution> _solutionProvider;

		public MetricsRepository(ICodeMetricsCalculator metricsCalculator, IProvider<string, ISolution> solutionProvider)
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
						var solution = _solutionProvider.Get(path);
						var tasks = solution.Projects.Select(x => LoadMetrics(solution, x));
						return await Task.WhenAll(tasks);
					});
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				_metrics.Clear();
			}
		}

		private async Task<IProjectMetric> LoadMetrics(ISolution solution, IProject project)
		{
			if (project == null)
			{
				return null;
			}

			var metrics = (await _metricsCalculator.Calculate(project)).ToArray();

			var referencedProjects = project.ProjectReferences
				.Select(x => solution.GetProject(x).AssemblyName)
				.Concat(project.MetadataReferences.Select(x => x.Display));

			return new ProjectMetric(project.Name, metrics, referencedProjects);
		}
	}
}