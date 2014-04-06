// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SolutionVertexRepository.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the SolutionVertexRepository type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Model
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

	public class SolutionVertexRepository : IVertexRepository
	{
		private readonly ICodeErrorRepository _codeErrorRepository;
		private readonly ConcurrentDictionary<string, Task<IEnumerable<IModelNode>>> _knownVertices = new ConcurrentDictionary<string, Task<IEnumerable<IModelNode>>>();
		private readonly IProjectMetricsRepository _metricsRepository;

		public SolutionVertexRepository(
			ICodeErrorRepository codeErrorRepository,
			IProjectMetricsRepository metricsRepository)
		{
			_codeErrorRepository = codeErrorRepository;
			_metricsRepository = metricsRepository;
		}

		public async Task<IEnumerable<IModelNode>> GetVertices(string solutionPath, CancellationToken cancellationToken)
		{
			var projectVertices = await _knownVertices.GetOrAdd(
				solutionPath,
				async path =>
				{
					var evaluationResults = (await _codeErrorRepository.GetErrors(solutionPath, cancellationToken)).ToArray();
					var projectMetrics = (await _metricsRepository.Get(solutionPath)).ToArray();
					var vertices = projectMetrics.Select(IProjectMetric => CreateProjectNode(IProjectMetric, projectMetrics, evaluationResults)).ToArray();

					return vertices;
				});
			return cancellationToken.IsCancellationRequested
					   ? Enumerable.Empty<IModelNode>()
					   : projectVertices;
		}

		private static ModelNode CreateProjectNode(
			IProjectMetric projectMetric,
			IProjectMetric[] projectMetrics,
			EvaluationResult[] evaluationResults)
		{
			var children = projectMetric.ReferencedProjects.Select(
				y =>
				{
					var couplings =
						projectMetric.NamespaceMetrics.SelectMany(x => x.ClassCouplings)
							.Where(x => x.Assembly == Path.GetFileNameWithoutExtension(y))
							.Select(x => new ModelNode(x.Namespace, NodeKind.Namespace, CodeQuality.Good, 0, 100, 0))
							.Cast<IModelNode>()
							.ToList();
					return new ModelNode(
						Path.GetFileNameWithoutExtension(y),
						NodeKind.Assembly,
						CodeQuality.Good,
						0,
						100,
						0,
						couplings);
				})
				.Concat(
					projectMetric.NamespaceMetrics.Select(
						namespaceMetric =>
						CreateNamespaceNode(
							namespaceMetric,
							projectMetrics,
							evaluationResults.Where(r => r.Namespace == namespaceMetric.Name))))
				.Merge()
				.ToList();
			return new ModelNode(
				projectMetric.Name,
				NodeKind.Assembly,
				evaluationResults.Where(x => x.ProjectName == projectMetric.Name).GetQuality(),
				projectMetric.LinesOfCode,
				projectMetric.MaintainabilityIndex,
				projectMetric.CyclomaticComplexity,
				children);
		}

		private static IModelNode CreateNamespaceNode(INamespaceMetric namespaceMetric, IProjectMetric[] projectMetrics, IEnumerable<EvaluationResult> reviews)
		{
			var references =
				namespaceMetric.ClassCouplings.Select(definition => CreateNamespaceReferenceNode(definition, projectMetrics)).ToArray();
			var children =
				namespaceMetric.TypeMetrics.Select(
					typeMetric => CreateTypeNodes(typeMetric, projectMetrics, reviews.Where(x => x.TypeName == typeMetric.Name)))
					.Concat(references)
					.Merge()
					.ToList();
			return new ModelNode(
				namespaceMetric.Name,
				NodeKind.Namespace,
				reviews.Where(x => x.Namespace == namespaceMetric.Name).GetQuality(),
				namespaceMetric.LinesOfCode,
				namespaceMetric.MaintainabilityIndex,
				namespaceMetric.CyclomaticComplexity,
				children);
		}

		private static IModelNode CreateTypeNodes(ITypeMetric typeMetric, IEnumerable<IProjectMetric> projectMetrics, IEnumerable<EvaluationResult> reviews)
		{
			var children = typeMetric.ClassCouplings
				.Select(definition => CreateTypeReferenceNode(definition, projectMetrics))
				.Merge()
				.ToList();
			return new ModelNode(
				typeMetric.Name,
				typeMetric.Kind.ToString(),
				reviews.GetQuality(),
				typeMetric.LinesOfCode,
				typeMetric.MaintainabilityIndex,
				typeMetric.CyclomaticComplexity,
				children);
		}

		private static IModelNode CreateTypeReferenceNode(ITypeDefinition definition, IEnumerable<IProjectMetric> projectMetrics)
		{
			var actualType = (from project in projectMetrics
							  where project.Name == definition.Assembly
							  from ns in project.NamespaceMetrics
							  where ns.Name == definition.Namespace
							  from t in ns.TypeMetrics
							  where t.Name == definition.TypeName
							  select t)
				.FirstOrDefault();

			return new StaticModelNode(
				string.Join(".", definition.Namespace, definition.TypeName),
				actualType == null ? NodeKind.Class : actualType.Kind.ToString().ToTitleCase(),
				CodeQuality.Good,
				actualType == null ? 0 : actualType.LinesOfCode,
				actualType == null ? 100 : actualType.MaintainabilityIndex,
				actualType == null ? 0 : actualType.CyclomaticComplexity,
				new List<IModelNode>());
		}

		private static IModelNode CreateNamespaceReferenceNode(ITypeDefinition definition, IEnumerable<IProjectMetric> projectMetrics)
		{
			var actualNs = (from project in projectMetrics
							where project.Name == definition.Assembly
							from ns in project.NamespaceMetrics
							where ns.Name == definition.Namespace
							select ns)
				.FirstOrDefault();

			var children = actualNs == null
							   ? new List<IModelNode> { new ModelNode(definition.TypeName, definition.Namespace, CodeQuality.Good, 0, 100, 0) }
							   : new List<IModelNode>();
			return new StaticModelNode(
				definition.Namespace,
				NodeKind.Namespace,
				CodeQuality.Good,
				actualNs == null ? 0 : actualNs.LinesOfCode,
				actualNs == null ? 100 : actualNs.MaintainabilityIndex,
				actualNs == null ? 0 : actualNs.CyclomaticComplexity,
				children);
		}
	}
}
