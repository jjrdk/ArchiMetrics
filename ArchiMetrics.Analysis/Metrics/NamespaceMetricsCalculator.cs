// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NamespaceMetricsCalculator.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the NamespaceMetricsCalculator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMetrics.Analysis.Metrics
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Common.Metrics;
	using Roslyn.Compilers.Common;

	internal sealed class NamespaceMetricsCalculator : SemanticModelMetricsCalculator
	{
		public NamespaceMetricsCalculator(ISemanticModel semanticModel)
			: base(semanticModel)
		{
		}

		public NamespaceMetric CalculateFrom(NamespaceDeclarationSyntaxInfo namespaceNode, IEnumerable<TypeMetric> metrics)
		{
			var typeMetrics = metrics.ToArray();
			var linesOfCode = typeMetrics.Sum(x => x.LinesOfCode);
			var source = typeMetrics.SelectMany(x => x.ClassCouplings)
						  .Distinct(TypeCouplingComparer.Default)
						  .OrderBy(x => x.ClassName)
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
