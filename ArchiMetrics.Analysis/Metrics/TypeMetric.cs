// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeMetric.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the TypeMetric type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Metrics
{
	using System.Collections.Generic;
	using System.Linq;
	using ArchiMetrics.Common;
	using ArchiMetrics.Common.Metrics;

	internal class TypeMetric : ITypeMetric
	{
		public TypeMetric(
			bool isAbstract,
			TypeMetricKind kind,
			AccessModifierKind accessModifier,
			IEnumerable<IMemberMetric> memberMetrics,
			int linesOfCode,
			int cyclomaticComplexity,
			double maintainabilityIndex,
			int depthOfInheritance,
			IEnumerable<ITypeCoupling> classCouplings,
			string name,
			int afferentCoupling,
			int efferentCoupling,
			double instability,
			IDocumentation documentation)
		{
			IsAbstract = isAbstract;
			Kind = kind;
			AccessModifier = accessModifier;
			MemberMetrics = memberMetrics;
			LinesOfCode = linesOfCode;
			CyclomaticComplexity = cyclomaticComplexity;
			MaintainabilityIndex = maintainabilityIndex;
			DepthOfInheritance = depthOfInheritance;
			ClassCouplings = classCouplings.AsArray();
			Name = name;
			AfferentCoupling = afferentCoupling;
			EfferentCoupling = efferentCoupling;
			Instability = instability;
			Documentation = documentation;
		}

		public bool IsAbstract { get; private set; }

		public string Name { get; private set; }

		public int AfferentCoupling { get; private set; }

		public int EfferentCoupling { get; private set; }

		public double Instability { get; private set; }

		public IDocumentation Documentation { get; private set; }

		public TypeMetricKind Kind { get; private set; }

		public AccessModifierKind AccessModifier { get; private set; }

		public IEnumerable<IMemberMetric> MemberMetrics { get; private set; }

		public int LinesOfCode { get; private set; }

		public int CyclomaticComplexity { get; private set; }

		public double MaintainabilityIndex { get; private set; }

		public int DepthOfInheritance { get; private set; }

		public IEnumerable<ITypeCoupling> ClassCouplings { get; private set; }

		public int ClassCoupling
		{
			get { return ClassCouplings.Count(); }
		}
	}
}
