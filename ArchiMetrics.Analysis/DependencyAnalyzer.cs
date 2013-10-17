// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DependencyAnalyzer.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
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
	public class DependencyAnalyzer
	{
		private readonly ComparableComparer<TypeDefinition> _comparer;

		public DependencyAnalyzer()
		{
			_comparer = new ComparableComparer<TypeDefinition>();
		}

		public Task<IEnumerable<DependencyChain>> GetCircularReferences(IEnumerable<EdgeItem> items)
		{
			var edgeItems = items.ToArray();
			return Task.Factory.StartNew(() => edgeItems.SelectMany(e => GetDependencyChain(new DependencyChain(Enumerable.Empty<EdgeItem>(), e, e), edgeItems))
				.Where(c => c.IsCircular)
				.Distinct());
		}

		public async Task<IEnumerable<TypeDefinition>> GetUsedTypes(IDocument document)
		{
			var semanticModel = await document.GetSemanticModelAsync();
			var nodes = (await document.GetSyntaxRootAsync())
				.DescendantNodes()
				.Select(x => GetUsedType(x, semanticModel))
				.Where(x => x != null)
				.Distinct(_comparer);

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

		private static IEnumerable<DependencyChain> GetDependencyChain(DependencyChain chain, EdgeItem[] source)
		{
			return chain.IsCircular
					   ? new[] { chain }
					   : source.Where(chain.IsContinuation).SelectMany(i => GetDependencyChain(new DependencyChain(chain.ReferenceChain, chain.Root, i), source));
		}
	}
}
