namespace ArchiMeter.Reports.Metrics
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Core.Metrics;
	using Roslyn.Compilers.CSharp;
	using Roslyn.Compilers.Common;

	internal sealed class TypeMetricsCalculator : SemanticModelMetricsCalculator
	{
		// Methods
		public TypeMetricsCalculator(ISemanticModel semanticModel)
			: base(semanticModel)
		{
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

		private IEnumerable<string> CalculateClassCoupling(TypeDeclarationSyntax type, IEnumerable<MemberMetric> memberMetrics)
		{
			IEnumerable<string> second = new TypeClassCouplingAnalyzer(Model).Calculate(type);
			return memberMetrics.SelectMany(x => x.ClassCouplings).Concat(second).Distinct().OrderBy(x => x).ToArray();
		}

		private int CalculateDepthOfInheritance(TypeDeclarationSyntax type)
		{
			var analyzer = new DepthOfInheritanceAnalyzer(Model);
			return analyzer.Calculate(type);
		}

		public TypeMetric CalculateFrom(TypeDeclarationSyntaxInfo typeNode, IEnumerable<MemberMetric> metrics)
		{
			var memberMetrics = metrics.ToArray();
			var type = (TypeDeclarationSyntax)typeNode.Syntax;
			var hasDefaultConstructor = !type.Members.Any(SyntaxKind.ConstructorDeclaration);
			var metricKind = GetMetricKind(type);
			var source = this.CalculateClassCoupling(type, memberMetrics);
			var depthOfInheritance = CalculateDepthOfInheritance(type);
			var cyclomaticComplexity = memberMetrics.Sum(x => x.CyclomaticComplexity) + (hasDefaultConstructor ? 1 : 0);
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

		private static TypeMetricKind GetMetricKind(TypeDeclarationSyntax type)
		{
			bool flag = type is InterfaceDeclarationSyntax;
			bool flag2 = type is ClassDeclarationSyntax;
			bool flag3 = type is StructDeclarationSyntax;
			if (flag2)
			{
				return TypeMetricKind.Class;
			}
			if (flag)
			{
				return TypeMetricKind.Interface;
			}
			if (flag3)
			{
				return TypeMetricKind.Struct;
			}
			return TypeMetricKind.Unknown;
		}
	}
}