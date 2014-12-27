// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeMetricsCalculator.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the TypeMetricsCalculator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Metrics
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using ArchiMetrics.Common;
	using ArchiMetrics.Common.Metrics;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;

	internal sealed class TypeMetricsCalculator : SemanticModelMetricsCalculator
	{
		private readonly Solution _solution;

		public TypeMetricsCalculator(SemanticModel semanticModel, Solution solution)
			: base(semanticModel)
		{
			_solution = solution;
		}

		public async Task<ITypeMetric> CalculateFrom(TypeDeclarationSyntaxInfo typeNode, IEnumerable<IMemberMetric> metrics)
		{
			var memberMetrics = metrics.AsArray();
			var type = typeNode.Syntax;
			var symbol = Model.GetDeclaredSymbol(type);
			var metricKind = GetMetricKind(type);
			var source = CalculateClassCoupling(type, memberMetrics);
			var depthOfInheritance = CalculateDepthOfInheritance(type);
			var cyclomaticComplexity = memberMetrics.Sum(x => x.CyclomaticComplexity);
			var linesOfCode = memberMetrics.Sum(x => x.LinesOfCode);
			var maintainabilityIndex = CalculateAveMaintainabilityIndex(memberMetrics);
			var afferentCoupling = await CalculateAfferentCoupling(type);
			var efferentCoupling = GetEfferentCoupling(type, symbol);
			var instability = (double)efferentCoupling / (efferentCoupling + afferentCoupling);
			var modifier = GetAccessModifier(type.Modifiers);
			return new TypeMetric(
				symbol.IsAbstract,
				metricKind,
				modifier,
				memberMetrics,
				linesOfCode,
				cyclomaticComplexity,
				maintainabilityIndex,
				depthOfInheritance,
				source,
				type.GetName(),
				afferentCoupling,
				efferentCoupling,
				instability);
		}

		private int GetEfferentCoupling(SyntaxNode classDeclaration, ISymbol sourceSymbol)
		{
			var typeSyntaxes = classDeclaration.DescendantNodesAndSelf().OfType<TypeSyntax>();
			var commonSymbolInfos = typeSyntaxes.Select(x => Model.GetSymbolInfo(x)).AsArray();
			var members = commonSymbolInfos
				.Select(x => x.Symbol)
				.Where(x => x != null)
				.Select(x =>
					{
						var typeSymbol = x as ITypeSymbol;
						return typeSymbol == null ? x.ContainingType : x;
					})
				.Cast<ITypeSymbol>()
				.WhereNotNull()
				.DistinctBy(x => x.ToDisplayString())
				.Count(x => !x.Equals(sourceSymbol));

			return members;
		}

		private async Task<int> CalculateAfferentCoupling(SyntaxNode node)
		{
			try
			{
				if (_solution == null)
				{
					return 0;
				}

				if (node.SyntaxTree != Model.SyntaxTree)
				{
					return 0;
				}

				var symbol = Model.GetDeclaredSymbol(node);
				var referenceTasks = symbol == null
										 ? Task.FromResult(0)
										 : _solution.FindReferences(symbol).ContinueWith(t => t.Exception != null ? 0 : t.Result.Locations.Count());

				return await referenceTasks.ConfigureAwait(false);
			}
			catch
			{
				// Some types are not present in syntax tree because they have been created for metrics calculation.
				return 0;
			}
		}

		private static double CalculateAveMaintainabilityIndex(IEnumerable<IMemberMetric> memberMetrics)
		{
			var source = memberMetrics.Select(x => new Tuple<int, double>(x.LinesOfCode, x.MaintainabilityIndex)).AsArray();
			if (source.Any())
			{
				var totalLinesOfCode = source.Sum(x => x.Item1);
				return totalLinesOfCode == 0 ? 100.0 : source.Sum(x => x.Item1 * x.Item2) / totalLinesOfCode;
			}

			return 100.0;
		}

		private static TypeMetricKind GetMetricKind(TypeDeclarationSyntax type)
		{
			switch (type.CSharpKind())
			{
				case SyntaxKind.ClassDeclaration:
					return TypeMetricKind.Class;
				case SyntaxKind.StructDeclaration:
					return TypeMetricKind.Struct;
				case SyntaxKind.InterfaceDeclaration:
					return TypeMetricKind.Interface;
				default:
					return TypeMetricKind.Unknown;
			}
		}

		private AccessModifierKind GetAccessModifier(SyntaxTokenList tokenList)
		{
			if (tokenList.Any(SyntaxKind.PublicKeyword))
			{
				return AccessModifierKind.Public;
			}

			if (tokenList.Any(SyntaxKind.PrivateKeyword))
			{
				return AccessModifierKind.Private;
			}

			return AccessModifierKind.Internal;
		}

		private IEnumerable<ITypeCoupling> CalculateClassCoupling(TypeDeclarationSyntax type, IEnumerable<IMemberMetric> memberMetrics)
		{
			var second = new TypeClassCouplingAnalyzer(Model).Calculate(type);
			return memberMetrics.SelectMany(x => x.ClassCouplings)
				.Concat(second)
				.GroupBy(x => x.ToString())
				.Select(x => new TypeCoupling(x.First().TypeName, x.First().Namespace, x.First().Assembly, x.SelectMany(y => y.UsedMethods), x.SelectMany(y => y.UsedProperties), x.SelectMany(y => y.UsedEvents)))
				.OrderBy(x => x.TypeName)
				.AsArray();
		}

		private int CalculateDepthOfInheritance(TypeDeclarationSyntax type)
		{
			var analyzer = new DepthOfInheritanceAnalyzer(Model);
			return analyzer.Calculate(type);
		}
	}
}
