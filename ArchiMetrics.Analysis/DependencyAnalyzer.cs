// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DependencyAnalyzer.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the DependencyAnalyzer class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using ArchiMetrics.Analysis.Metrics;
	using ArchiMetrics.Common.Metrics;
	using ArchiMetrics.Common.Structure;
	using Roslyn.Compilers.Common;
	using Roslyn.Services;

	/// <summary>
	/// Defines the DependencyAnalyzer class.
	/// </summary>
	public static class DependencyAnalyzer
	{
		private static readonly ComparableComparer<TypeDefinition> Comparer = new ComparableComparer<TypeDefinition>();

		public static Task<IEnumerable<DependencyChain>> GetCircularReferences(IEnumerable<IProjectMetric> projectMetrics)
		{
			var edges =
				projectMetrics
					.SelectMany(
						p => p.ReferencedProjects.Select(
							r => new EdgeItemBase
							{
								Dependant = p.Name,
								Dependency = r,
								MergedEdges = 0
							}))
					.ToArray();

			return GetCircularReferences(edges);
		}

		public static Task<IEnumerable<DependencyChain>> GetCircularReferences(IEnumerable<EdgeItemBase> items)
		{
			var edgeItems = items.WhereNotNull().ToArray();
			return Task.Factory.StartNew(
				() => edgeItems
					.AsParallel()
					.SelectMany(e => GetDependencyChain(new DependencyChain(Enumerable.Empty<MetricsEdgeItem>(), e, e), edgeItems))
					.Where(c => c.IsCircular)
					.AsSequential()
					.Distinct());
		}

		public static async Task<IEnumerable<TypeDefinition>> GetUsedTypes(IDocument document)
		{
			var semanticModel = await document.GetSemanticModelAsync();
			var nodes = (await document.GetSyntaxRootAsync())
				.DescendantNodes()
				.Select(x => GetUsedType(x, semanticModel))
				.WhereNotNull()
				.Distinct(Comparer);

			return nodes;
		}

		private static TypeDefinition GetUsedType(CommonSyntaxNode node, ISemanticModel semanticModel)
		{
			var typeSymbol = semanticModel.GetTypeInfo(node).Type;
			if (typeSymbol == null || typeSymbol.TypeKind == CommonTypeKind.Error)
			{
				return null;
			}

			var qualifiedName = typeSymbol.GetQualifiedName();

			return qualifiedName;
		}

		private static IEnumerable<DependencyChain> GetDependencyChain(DependencyChain chain, IEnumerable<EdgeItemBase> source)
		{
			return chain.IsCircular
					   ? new[] { chain }
					   : source.Where(chain.IsContinuation).SelectMany(i => GetDependencyChain(new DependencyChain(chain.ReferenceChain, chain.Root, i), source));
		}
	}
}
