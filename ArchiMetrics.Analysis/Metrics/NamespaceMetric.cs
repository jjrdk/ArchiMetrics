// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NamespaceMetric.cs" company="Reimers.dk">
//   Copyright � Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
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
	using ArchiMetrics.Common;
	using ArchiMetrics.Common.Metrics;

	internal class NamespaceMetric : INamespaceMetric
	{
		public NamespaceMetric(
			double maintainabilityIndex,
			int cyclomaticComplexity,
			int linesOfCode,
			int sourceLinesOfCode,
			IEnumerable<ITypeCoupling> classCouplings,
			int depthOfInheritance,
			string name,
			IEnumerable<ITypeMetric> typeMetrics)
		{
			MaintainabilityIndex = maintainabilityIndex;
			CyclomaticComplexity = cyclomaticComplexity;
			LinesOfCode = linesOfCode;
			SourceLinesOfCode = sourceLinesOfCode;
			ClassCouplings = classCouplings;
			DepthOfInheritance = depthOfInheritance;
			Name = name;
			TypeMetrics = typeMetrics.AsArray();
			Abstractness = TypeMetrics.Count(x => x.IsAbstract) / (double)TypeMetrics.Count();
		}

		public int SourceLinesOfCode { get; private set; }

		public double Abstractness { get; private set; }

		public double MaintainabilityIndex { get; private set; }

		public int CyclomaticComplexity { get; private set; }

		public int LinesOfCode { get; private set; }

		public IEnumerable<ITypeCoupling> ClassCouplings { get; private set; }

		public int DepthOfInheritance { get; private set; }

		public string Name { get; private set; }

		public IEnumerable<ITypeMetric> TypeMetrics { get; private set; }
	}
}
