// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectMetric.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ProjectMetric type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Metrics
{
	using System.Collections.Generic;
	using System.Linq;
	using ArchiMetrics.Common.Metrics;

	internal class ProjectMetric : IProjectMetric
	{
		private static readonly IEqualityComparer<TypeCoupling> Comparer = new ComparableComparer<TypeCoupling>();

		public ProjectMetric(string name, IEnumerable<INamespaceMetric> namespaceMetrics, IEnumerable<string> referencedProjects, double relationalCohesion)
		{
			Name = name;
			RelationalCohesion = relationalCohesion;
			ReferencedProjects = referencedProjects.ToArray();
			NamespaceMetrics = namespaceMetrics.ToArray();
			LinesOfCode = NamespaceMetrics.Sum(x => x.LinesOfCode);
			MaintainabilityIndex = LinesOfCode == 0 ? 100 : NamespaceMetrics.Sum(x => x.MaintainabilityIndex * x.LinesOfCode) / LinesOfCode;
			CyclomaticComplexity = LinesOfCode == 0 ? 0 : NamespaceMetrics.Sum(x => x.CyclomaticComplexity * x.LinesOfCode) / LinesOfCode;
			ClassCouplings = NamespaceMetrics.SelectMany(x => x.ClassCouplings).Distinct(Comparer).ToArray();
		}

		public int LinesOfCode { get; private set; }

		public double MaintainabilityIndex { get; private set; }

		public int CyclomaticComplexity { get; private set; }

		public string Name { get; private set; }

		public double RelationalCohesion { get; set; }

		public IEnumerable<string> ReferencedProjects { get; private set; }

		public IEnumerable<INamespaceMetric> NamespaceMetrics { get; private set; }

		public IEnumerable<TypeCoupling> ClassCouplings { get; private set; }
	}
}