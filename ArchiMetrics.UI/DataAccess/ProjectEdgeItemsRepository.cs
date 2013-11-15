// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectEdgeItemsRepository.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ProjectEdgeItemsRepository type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.UI.DataAccess
{
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using ArchiMetrics.Common;
	using ArchiMetrics.Common.CodeReview;
	using ArchiMetrics.Common.Metrics;
	using ArchiMetrics.Common.Structure;
	using Roslyn.Services;

	public class ProjectEdgeItemsRepository : CodeEdgeItemsRepository
	{
		private readonly ConcurrentDictionary<string, Task<ProjectReference[]>> _projectReferences = new ConcurrentDictionary<string, Task<ProjectReference[]>>();
		private readonly IProvider<string, ISolution> _solutionProvider;
		private readonly IMetricsRepository _metricsRepository;

		public ProjectEdgeItemsRepository(
			IProvider<string, ISolution> solutionProvider,
			ICodeErrorRepository codeErrorRepository,
			IMetricsRepository metricsRepository)
			: base(codeErrorRepository)
		{
			_solutionProvider = solutionProvider;
			_metricsRepository = metricsRepository;
		}

		protected override async Task<IEnumerable<MetricsEdgeItem>> CreateEdges(string path, IEnumerable<EvaluationResult> source, CancellationToken cancellationToken)
		{
			var references = await GetProjectReferences(path, cancellationToken);
			if (cancellationToken.IsCancellationRequested)
			{
				return Enumerable.Empty<MetricsEdgeItem>();
			}

			var results = source.GroupBy(er => er.ProjectPath).ToArray();
			var metrics = (await GetCodeMetrics(path, cancellationToken)).ToDictionary(m => m.ProjectPath);
			var edges = references
				.TakeWhile(x => !cancellationToken.IsCancellationRequested)
				.SelectMany(pr => pr.ProjectReferences.Select(r => CreateEdgeItem(pr.Name, r.Key, pr.ProjectPath, metrics[pr.ProjectPath], metrics[r.Value], results))
					.Concat(pr.AssemblyReferences.Select(a => CreateEdgeItem(pr.Name, a, pr.ProjectPath, metrics[pr.ProjectPath], new ProjectCodeMetrics(), new EvaluationResult[0].GroupBy(x => x.ProjectPath)))))
					.Concat(references.Select(r => CreateEdgeItem(r.Name, r.Name, r.ProjectPath, metrics[r.ProjectPath], metrics[r.ProjectPath], results)))
					.ToArray();

			return cancellationToken.IsCancellationRequested
					   ? Enumerable.Empty<MetricsEdgeItem>()
					   : edges;
		}

		private Task<ProjectReference[]> GetProjectReferences(string solutionPath, CancellationToken cancellationToken)
		{
			return _projectReferences.GetOrAdd(
				solutionPath,
				path => Task.Factory.StartNew(() => GetProjectDependencies(path).ToArray(), cancellationToken, TaskCreationOptions.PreferFairness, PriorityScheduler.AboveNormal));
		}

		private async Task<IEnumerable<ProjectCodeMetrics>> GetCodeMetrics(string path, CancellationToken cancellationToken)
		{
			var solution = _solutionProvider.Get(path);
			var metricTasks = solution.Projects
				.TakeWhile(x => !cancellationToken.IsCancellationRequested)
				.Select(x => _metricsRepository.Get(x.FilePath, path));
			var metrics = await Task.WhenAll(metricTasks);

			return cancellationToken.IsCancellationRequested
				? Enumerable.Empty<ProjectCodeMetrics>()
				: metrics.Where(x => x != null).ToArray();
		}

		private IEnumerable<ProjectReference> GetProjectDependencies(string path)
		{
			var solution = _solutionProvider.Get(path);

			return solution.Projects
				.Select(
					p => new ProjectReference
						 {
							 ProjectPath = p.FilePath,
							 Version = p.GetVersion().ToString(),
							 Name = p.Name,
							 ProjectReferences = p.ProjectReferences.Select(
								 pr =>
								 {
									 var project = solution.GetProject(pr);
									 return new KeyValuePair<string, string>(project.Name, project.FilePath);
								 }),
							 AssemblyReferences = p.MetadataReferences.Select(m => Path.GetFileNameWithoutExtension(m.Display))
						 });
		}
	}
}
