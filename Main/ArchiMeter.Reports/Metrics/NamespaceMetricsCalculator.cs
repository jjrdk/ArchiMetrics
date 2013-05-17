namespace ArchiMeter.Reports.Metrics
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Core.Metrics;
	using Roslyn.Compilers.Common;

	internal sealed class NamespaceMetricsCalculator : SemanticModelMetricsCalculator
	{
		// Methods
		public NamespaceMetricsCalculator(ISemanticModel semanticModel)
			: base(semanticModel)
		{
		}

		public NamespaceMetric CalculateFrom(NamespaceDeclarationSyntaxInfo namespaceNode, IEnumerable<TypeMetric> metrics)
		{
			var typeMetrics = metrics.ToArray();
			var linesOfCode = typeMetrics.Sum(x => x.LinesOfCode);
			var source = typeMetrics.SelectMany(x => x.ClassCouplings)
						  .Distinct()
						  .OrderBy(x => x)
						  .ToList();
			var maintainabilitySource = typeMetrics.Select(x => new Tuple<int, double>(x.LinesOfCode, x.MaintainabilityIndex)).ToArray();
			var maintainabilityIndex = linesOfCode > 0 && maintainabilitySource.Any() ? maintainabilitySource.Sum(x => x.Item1 * x.Item2) / linesOfCode : 100.0;
			var cyclomaticComplexity = typeMetrics.Sum(x => x.CyclomaticComplexity);
			var depthOfInheritance = typeMetrics.Any() ? typeMetrics.Max(x => x.DepthOfInheritance) : 0;
			return new NamespaceMetric(
				maintainabilityIndex,
				cyclomaticComplexity,
				linesOfCode,
				source,
				depthOfInheritance,
				namespaceNode.Name,
				typeMetrics);
		}
	}
}