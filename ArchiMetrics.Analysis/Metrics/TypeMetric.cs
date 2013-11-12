// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeMetric.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
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
	using ArchiMetrics.Common.Metrics;

	internal class TypeMetric : ITypeMetric
	{
		public TypeMetric(
			TypeMetricKind kind, 
			IEnumerable<IMemberMetric> memberMetrics, 
			int linesOfCode, 
			int cyclomaticComplexity, 
			double maintainabilityIndex, 
			int depthOfInheritance, 
			IEnumerable<TypeCoupling> classCouplings, 
			string name)
		{
			Kind = kind;
			MemberMetrics = memberMetrics;
			LinesOfCode = linesOfCode;
			CyclomaticComplexity = cyclomaticComplexity;
			MaintainabilityIndex = maintainabilityIndex;
			DepthOfInheritance = depthOfInheritance;
			ClassCouplings = classCouplings.ToArray();
			Name = name;
		}

		public string Name { get; private set; }

		public TypeMetricKind Kind { get; private set; }

		public IEnumerable<IMemberMetric> MemberMetrics { get; private set; }

		public int LinesOfCode { get; private set; }

		public int CyclomaticComplexity { get; private set; }

		public double MaintainabilityIndex { get; private set; }

		public int DepthOfInheritance { get; private set; }

		public IEnumerable<TypeCoupling> ClassCouplings { get; private set; }

		public int ClassCoupling
		{
			get { return ClassCouplings.Count(); }
		}
	}
}
