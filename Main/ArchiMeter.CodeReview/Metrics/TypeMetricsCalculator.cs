// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeMetricsCalculator.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the TypeMetricsCalculator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMeter.CodeReview.Metrics
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Common.Metrics;
	using Roslyn.Compilers.CSharp;
	using Roslyn.Compilers.Common;

	internal sealed class TypeMetricsCalculator : SemanticModelMetricsCalculator
	{
		// Methods
		public TypeMetricsCalculator(ISemanticModel semanticModel)
			: base(semanticModel)
		{
		}

		public TypeMetric CalculateFrom(TypeDeclarationSyntaxInfo typeNode, IEnumerable<MemberMetric> metrics)
		{
			var memberMetrics = metrics.ToArray();
			var type = (TypeDeclarationSyntax)typeNode.Syntax;
			var metricKind = GetMetricKind(type);
			var source = CalculateClassCoupling(type, memberMetrics);
			var depthOfInheritance = CalculateDepthOfInheritance(type);
			var cyclomaticComplexity = memberMetrics.Sum(x => x.CyclomaticComplexity);
			var linesOfCode = memberMetrics.Sum(x => x.LinesOfCode);
			var maintainabilityIndex = CalculateAveMaintainabilityIndex(memberMetrics);
			return new TypeMetric(
				metricKind, 
				memberMetrics, 
				linesOfCode, 
				cyclomaticComplexity, 
				maintainabilityIndex, 
				depthOfInheritance, 
				source, 
				TypeNameResolver.GetName(type));
		}

		private static double CalculateAveMaintainabilityIndex(IEnumerable<MemberMetric> memberMetrics)
		{
			var source = memberMetrics.Select(x => new Tuple<int, double>(x.LinesOfCode, x.MaintainabilityIndex)).ToArray();
			if (source.Any())
			{
				var totalLinesOfCode = source.Sum(x => x.Item1);
				return totalLinesOfCode == 0 ? 100.0 : source.Sum(x => x.Item1 * x.Item2) / totalLinesOfCode;
			}

			return 100.0;
		}

		private static TypeMetricKind GetMetricKind(TypeDeclarationSyntax type)
		{
			switch (type.Kind)
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

		private IEnumerable<TypeCoupling> CalculateClassCoupling(TypeDeclarationSyntax type, IEnumerable<MemberMetric> memberMetrics)
		{
			var second = new TypeClassCouplingAnalyzer(Model).Calculate(type);
			return memberMetrics.SelectMany(x => x.ClassCouplings)
				.Concat(second)
				.Distinct(TypeCouplingComparer.Default)
				.OrderBy(x => x.ClassName)
				.ToArray();
		}

		private int CalculateDepthOfInheritance(TypeDeclarationSyntax type)
		{
			var analyzer = new DepthOfInheritanceAnalyzer(Model);
			return analyzer.Calculate(type);
		}
	}
}