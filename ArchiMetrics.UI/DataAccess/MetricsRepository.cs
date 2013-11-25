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
	using System.Linq;
	using System.Threading.Tasks;
	using ArchiMetrics.Common;
	using ArchiMetrics.Common.Metrics;
	using ArchiMetrics.Common.Structure;
	using Roslyn.Services;

	internal class MetricsRepository : IProjectMetricsRepository, IDisposable
	{
		private readonly ConcurrentDictionary<string, Task<ProjectCodeMetrics>> _metrics = new ConcurrentDictionary<string, Task<ProjectCodeMetrics>>();
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

		public Task<ProjectCodeMetrics> Get(string key, string solutionPath)
		{
			var solution = _solutionProvider.Get(solutionPath);
			var project = solution.Projects.FirstOrDefault(x => x.FilePath == key);
			if (project == null)
			{
				return Task.FromResult(new ProjectCodeMetrics());
			}

			return Get(project);
		}

		public Task<ProjectCodeMetrics> Get(IProject project)
		{
			return _metrics.GetOrAdd(
				project.FilePath,
				async s => await LoadMetrics(project, s));
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				_metrics.Clear();
			}
		}

		private async Task<ProjectCodeMetrics> LoadMetrics(IProject project, string s)
		{
			var metrics = (await _metricsCalculator.Calculate(project)).ToArray();

			var linesOfCode = metrics.Sum(x => x.LinesOfCode);
			return new ProjectCodeMetrics
			{
				Metrics = metrics,
				Project = project.Name,
				ProjectPath = s,
				Version = project.GetVersion().ToString(),
				LinesOfCode = linesOfCode,
				DepthOfInheritance = linesOfCode > 0 ? (int)metrics.Average(x => x.DepthOfInheritance) : 0,
				CyclomaticComplexity = linesOfCode > 0 ? metrics.Sum(x => x.CyclomaticComplexity * x.LinesOfCode) / linesOfCode : 0,
				MaintainabilityIndex = linesOfCode > 0 ? metrics.Sum(x => x.MaintainabilityIndex * x.LinesOfCode) / linesOfCode : 0
			};
		}
	}
}