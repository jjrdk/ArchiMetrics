// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NamespaceMetric.cs" company="Reimers.dk">
//   Copyright © Matthias Friedrich, Reimers.dk 2014
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the NamespaceMetric type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Metrics
{
	using System.Collections.Generic;
	using System.Linq;
	using Common;
	using Common.Metrics;

    internal class NamespaceMetric : INamespaceMetric
	{
		public NamespaceMetric(
			double maintainabilityIndex,
			int cyclomaticComplexity,
			int linesOfCode,
			IEnumerable<ITypeCoupling> classCouplings,
			int depthOfInheritance,
			string name,
			IEnumerable<ITypeMetric> typeMetrics,
			IDocumentation documentation)
		{
			MaintainabilityIndex = maintainabilityIndex;
			CyclomaticComplexity = cyclomaticComplexity;
			LinesOfCode = linesOfCode;
			ClassCouplings = classCouplings.AsArray();
			DepthOfInheritance = depthOfInheritance;
			Name = name;
			Documentation = documentation;
			TypeMetrics = typeMetrics.AsArray();
			Abstractness = TypeMetrics.Count(x => x.IsAbstract) / (double)TypeMetrics.Count();
		}

		public double Abstractness { get; }

		public double MaintainabilityIndex { get; }

		public int CyclomaticComplexity { get; }

		public int LinesOfCode { get; }

		public IEnumerable<ITypeCoupling> ClassCouplings { get; }

		public int DepthOfInheritance { get; }

		public string Name { get; }

		public IDocumentation Documentation { get; }

		public IEnumerable<ITypeMetric> TypeMetrics { get; }
	}
}
