// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NamespaceMetricsCalculator.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
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
	using ArchiMetrics.Common;
	using ArchiMetrics.Common.Metrics;
	using Microsoft.CodeAnalysis;

	internal sealed class NamespaceMetricsCalculator : SemanticModelMetricsCalculator
	{
		public NamespaceMetricsCalculator(SemanticModel semanticModel)
			: base(semanticModel)
		{
		}

		public INamespaceMetric CalculateFrom(NamespaceDeclarationSyntaxInfo namespaceNode, IEnumerable<ITypeMetric> metrics)
		{
			const string DocumentationTypeName = "NamespaceDoc";
			var typeMetrics = metrics.AsArray();
			var documentationType = typeMetrics.FirstOrDefault(x => x.Name == DocumentationTypeName);
			IDocumentation documentation = null;
			if (documentationType != null)
			{
				documentation = documentationType.Documentation;
				typeMetrics = typeMetrics.Where(x => x.Name != DocumentationTypeName).AsArray();
			}

			var linesOfCode = typeMetrics.Sum(x => x.LinesOfCode);
			var source = typeMetrics.SelectMany(x => x.ClassCouplings)
						  .GroupBy(x => x.ToString())
						  .Select(x => new TypeCoupling(x.First().TypeName, x.First().Namespace, x.First().Assembly, x.SelectMany(y => y.UsedMethods), x.SelectMany(y => y.UsedProperties), x.SelectMany(y => y.UsedEvents)))
						  .Where(x => x.Namespace != namespaceNode.Name)
						  .OrderBy(x => x.Assembly + x.Namespace + x.TypeName)
						  .AsArray();
			var maintainabilitySource = typeMetrics.Select(x => new Tuple<int, double>(x.LinesOfCode, x.MaintainabilityIndex)).AsArray();
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
				typeMetrics,
				documentation);
		}
	}
}
