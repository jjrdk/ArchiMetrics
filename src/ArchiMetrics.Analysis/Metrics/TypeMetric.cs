// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeMetric.cs" company="Reimers.dk">
//   Copyright © Matthias Friedrich, Reimers.dk 2014
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
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
	using Common;
	using Common.Metrics;

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
			ITypeDocumentation documentation)
		{
			IsAbstract = isAbstract;
			Kind = kind;
			AccessModifier = accessModifier;
			MemberMetrics = memberMetrics.AsArray();
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

		public bool IsAbstract { get; }

		public string Name { get; }

		public int AfferentCoupling { get; }

		public int EfferentCoupling { get; }

		public double Instability { get; }

		public ITypeDocumentation Documentation { get; }

		public TypeMetricKind Kind { get; }

		public AccessModifierKind AccessModifier { get; }

		public IEnumerable<IMemberMetric> MemberMetrics { get; }

		public int LinesOfCode { get; }

		public int CyclomaticComplexity { get; }

		public double MaintainabilityIndex { get; }

		public int DepthOfInheritance { get; }

		public IEnumerable<ITypeCoupling> ClassCouplings { get; }

		public int ClassCoupling
		{
			get { return ClassCouplings.Count(); }
		}
	}
}
