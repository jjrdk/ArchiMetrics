// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectEdgeItemsRepository.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ProjectEdgeItemsRepository type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMetrics.Data.DataAccess
{
	using System;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Threading.Tasks;
	using Common;
	using Common.Metrics;
	using Roslyn.Services;

	public class ProjectEdgeItemsRepository : CodeEdgeItemsRepository
	{
		private readonly ISolutionEdgeItemsRepositoryConfig _config;
		private readonly ConcurrentDictionary<string, Task<ProjectCodeMetrics>> _metrics = new ConcurrentDictionary<string, Task<ProjectCodeMetrics>>();
		private readonly ICodeMetricsCalculator _metricsCalculator;
		private readonly ConcurrentDictionary<string, Task<ProjectReference[]>> _projectReferences = new ConcurrentDictionary<string, Task<ProjectReference[]>>();
		private readonly IProvider<string, ISolution> _solutionProvider;

		public ProjectEdgeItemsRepository(
			ISolutionEdgeItemsRepositoryConfig config,
			IProvider<string, ISolution> solutionProvider,
			ICodeErrorRepository codeErrorRepository,
			ICodeMetricsCalculator metricsCalculator)
			: base(config, codeErrorRepository)
		{
			_config = config;
			_solutionProvider = solutionProvider;
			_metricsCalculator = metricsCalculator;
		}

		protected override async Task<IEnumerable<EdgeItem>> CreateEdges(IEnumerable<EvaluationResult> source)
		{
			var results = source.GroupBy(er => er.ProjectPath).ToArray();
			var references = await GetProjectReferences();
			var metrics = (await GetCodeMetrics()).ToDictionary(m => m.ProjectPath);
			return references
				.SelectMany(pr => pr.ProjectReferences.Select(r => CreateEdgeItem(pr.Name, r.Key, pr.ProjectPath, metrics[pr.ProjectPath], metrics[r.Value], results))
					.Concat(pr.AssemblyReferences.Select(a => CreateEdgeItem(pr.Name, a, pr.ProjectPath, metrics[pr.ProjectPath], new ProjectCodeMetrics(), new EvaluationResult[0].GroupBy(x => x.ProjectPath)))))
					.Concat(references.Select(r => CreateEdgeItem(r.Name, r.Name, r.ProjectPath, metrics[r.ProjectPath], metrics[r.ProjectPath], results)))
					.ToArray();
		}

		private Task<ProjectReference[]> GetProjectReferences()
		{
			return _projectReferences.GetOrAdd(
				_config.Path,
				path =>
				Task.Factory
					.StartNew(() =>
							  Directory.GetFiles(path, "*.sln", SearchOption.AllDirectories)
									   .Where(s => !s.Contains("QuickStart"))
									   .AsParallel()
									   .SelectMany(GetProjectDependencies)
									   .ToArray()));
		}

		private Task<IEnumerable<ProjectCodeMetrics>> GetCodeMetrics()
		{
			var metricTasks = Directory.GetFiles(_config.Path, "*.sln", SearchOption.AllDirectories)
									   .Where(s => !s.Contains("QuickStart"))
									   .AsParallel()
									   .Select(_solutionProvider.Get)
									   .SelectMany(s => s.Projects)
									   .Distinct(ProjectComparer.Default)
									   .Select(GetProjectMetrics);
			return Task.WhenAll(metricTasks).ContinueWith(task => task.Result.AsEnumerable());
		}

		private Task<ProjectCodeMetrics> GetProjectMetrics(IProject project)
		{
			return _metrics.GetOrAdd(
				project.FilePath,
				async s =>
				{
					try
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
					catch (Exception e)
					{
						Console.WriteLine(e.Message);
						return null;
					}
				});
		}

		private IEnumerable<ProjectReference> GetProjectDependencies(string path)
		{
			var solution = _solutionProvider.Get(path);

			return solution.Projects
						   .Select(p => new ProjectReference
											{
												ProjectPath = p.FilePath,
												Version = p.GetVersion().ToString(),
												Name = p.Name,
												ProjectReferences = p.ProjectReferences.Select(pr =>
																								   {
																									   var project = solution.GetProject(pr);
																									   return new KeyValuePair<string, string>(project.Name, project.FilePath);
																								   }),
												AssemblyReferences = p.MetadataReferences.Select(m => Path.GetFileNameWithoutExtension(m.Display))
											});
		}
	}
}
