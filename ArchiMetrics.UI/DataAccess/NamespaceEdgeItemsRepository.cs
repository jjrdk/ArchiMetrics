// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NamespaceEdgeItemsRepository.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the NamespaceEdgeItemsRepository type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.UI.DataAccess
{
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using ArchiMetrics.Common;
	using ArchiMetrics.Common.CodeReview;
	using ArchiMetrics.Common.Metrics;
	using ArchiMetrics.Common.Structure;
	using Roslyn.Compilers.CSharp;
	using Roslyn.Services;

	public class NamespaceEdgeItemsRepository : CodeEdgeItemsRepository
	{
		private readonly ConcurrentDictionary<string, IEnumerable<NamespaceReference>> _namespaceReferences = new ConcurrentDictionary<string, IEnumerable<NamespaceReference>>();
		private readonly IProvider<string, ISolution> _solutionProvider;
		private readonly IMetricsRepository _metricsProvider;

		public NamespaceEdgeItemsRepository(
			IProvider<string, ISolution> solutionProvider,
			IMetricsRepository metricsRepository,
			ICodeErrorRepository codeErrorRepository)
			: base(codeErrorRepository)
		{
			_solutionProvider = solutionProvider;
			_metricsProvider = metricsRepository;
		}

		protected override async Task<IEnumerable<MetricsEdgeItem>> CreateEdges(string path, IEnumerable<EvaluationResult> source, CancellationToken cancellationToken)
		{
			var results = source.GroupBy(x => x.Namespace).ToArray();
			var namespaceReferences = (await GetNamespaceReferences(path, cancellationToken)).ToArray();
			var metrics = (await GetCodeMetrics(path, namespaceReferences, cancellationToken))
				.SelectMany(x => x.Metrics.Select(_ => new
				{
					Name = _.Name,
					ProjectName = x.Project,
					ProjectPath = x.ProjectPath,
					ProjectVersion = x.Version,
					Metrics = _
				}))
				.Where(x => x.Metrics != null)
				.GroupBy(x => x.Name)
				.Select(
					metricsGroup =>
					{
						var linesOfCode = metricsGroup.Sum(x => x.Metrics.LinesOfCode);
						var first = metricsGroup.First();
						return new ProjectCodeMetrics
						{
							Metrics = metricsGroup.Select(x => x.Metrics).ToArray(),
							Project = first.Name,
							ProjectPath = first.ProjectPath,
							Version = first.ProjectVersion,
							LinesOfCode = linesOfCode,
							DepthOfInheritance = linesOfCode > 0 ? (int)metricsGroup.Average(x => x.Metrics.DepthOfInheritance) : 0,
							CyclomaticComplexity = linesOfCode > 0 ? metricsGroup.Sum(x => x.Metrics.CyclomaticComplexity * linesOfCode) / linesOfCode : 0,
							MaintainabilityIndex = linesOfCode > 0 ? metricsGroup.Sum(x => x.Metrics.MaintainabilityIndex * linesOfCode) / linesOfCode : 0
						};
					})
					.ToDictionary(x => x.Project);
			return namespaceReferences
				.GroupBy(n => n.Namespace)
				.Where(g => g.Any())
				.Select(g => new NamespaceReference
				{
					Namespace = g.Key,
					References = g.SelectMany(n => n.References.Distinct().ToArray())
				})
				.SelectMany(r => r.References.Select((x, i) => CreateEdgeItem(r.Namespace, x, r.Namespace, GetMetrics(metrics, r.Namespace), GetMetrics(metrics, x), results)))
				.ToArray();
		}

		private ProjectCodeMetrics GetMetrics(IDictionary<string, ProjectCodeMetrics> source, string namespaceName)
		{
			return source.ContainsKey(namespaceName)
					   ? source[namespaceName]
					   : new ProjectCodeMetrics();
		}

		private Task<IEnumerable<NamespaceReference>> GetNamespaceReferences(string solutionPath, CancellationToken cancellationToken)
		{
			return Task.Factory.StartNew(
				() => _namespaceReferences.GetOrAdd(
					solutionPath,
					path => _solutionProvider.Get(path)
								.Projects
								.Where(
									x =>
									{
										try
										{
											return x.HasDocuments;
										}
										catch
										{
											return false;
										}
									})
								.SelectMany(p => p.Documents)
								.Distinct(DocumentComparer.Default)
								.Select(d => new { Node = d.GetSyntaxTree().GetRoot() as SyntaxNode, Project = d.Project })
								.Select(
									x => new
											{
												Project = x.Project,
												LoC = GetLinesOfCode(x.Node),
												NamespaceNames = GetNamespaceNames(x.Node),
												Usings = GetUsings(x.Node)
											})
								.SelectMany(
									t => t.NamespaceNames.Select(
										s => new NamespaceReference
											 {
												 Namespace = s,
												 References = t.Usings.ToArray(),
												 ProjectPath = t.Project.FilePath,
												 ProjectVersion = t.Project.GetVersion().ToString()
											 }))
								.ToArray()
								.AsEnumerable()),
				cancellationToken);
		}

		private async Task<IEnumerable<ProjectCodeMetrics>> GetCodeMetrics(string path, IEnumerable<NamespaceReference> namespaceReferences, CancellationToken cancellationToken)
		{
			var metrics = namespaceReferences.Select(x => x.ProjectPath)
			.Distinct()
			.Select(x => _metricsProvider.Get(x, path))
			.ToArray();

			await Task.WhenAll(metrics);

			return cancellationToken.IsCancellationRequested
				? Enumerable.Empty<ProjectCodeMetrics>()
				: metrics.Select(x => x.Result);
		}
	}
}
