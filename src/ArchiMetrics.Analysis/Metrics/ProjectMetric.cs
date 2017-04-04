// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectMetric.cs" company="Reimers.dk">
//   Copyright © Matthias Friedrich, Reimers.dk 2014
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
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
	using Common;
	using Common.Metrics;

    internal class ProjectMetric : IProjectMetric
	{
		private static readonly IEqualityComparer<ITypeCoupling> Comparer = new ComparableComparer<ITypeCoupling>();

		public ProjectMetric(string name, IEnumerable<INamespaceMetric> namespaceMetrics, IEnumerable<string> referencedProjects, double relationalCohesion)
		{
			Name = name;
			RelationalCohesion = relationalCohesion;
			Dependencies = referencedProjects.AsArray();
			EfferentCoupling = Dependencies.Count();
			NamespaceMetrics = namespaceMetrics.AsArray();
			LinesOfCode = NamespaceMetrics.Sum(x => x.LinesOfCode);
			MaintainabilityIndex = LinesOfCode == 0 ? 100 : NamespaceMetrics.Sum(x => x.MaintainabilityIndex * x.LinesOfCode) / LinesOfCode;
			CyclomaticComplexity = LinesOfCode == 0 ? 0 : NamespaceMetrics.Sum(x => x.CyclomaticComplexity * x.LinesOfCode) / LinesOfCode;
			ClassCouplings = NamespaceMetrics.SelectMany(x => x.ClassCouplings).Where(x => x.Assembly != Name).Distinct(Comparer).AsArray();
			Dependendants = ClassCouplings.Select(x => x.Assembly)
				.Distinct()
				.AsArray();
			AfferentCoupling = Dependendants.Count();
			var typeMetrics = NamespaceMetrics.SelectMany(x => x.TypeMetrics).AsArray();
			Abstractness = typeMetrics.Count(x => x.IsAbstract) / (double)typeMetrics.Count();
		}

		public double Abstractness { get; }

		public int EfferentCoupling { get; }

		public int AfferentCoupling { get; }

		public int LinesOfCode { get; }

		public double MaintainabilityIndex { get; }

		public int CyclomaticComplexity { get; }

		public string Name { get; }

		public double RelationalCohesion { get; }

		public IEnumerable<string> Dependencies { get; }

		public IEnumerable<string> Dependendants { get; }

		public IEnumerable<INamespaceMetric> NamespaceMetrics { get; }

		public IEnumerable<ITypeCoupling> ClassCouplings { get; }
	}
}