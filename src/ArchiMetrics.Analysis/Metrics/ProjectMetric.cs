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
            AssemblyDependencies = referencedProjects.AsArray();
            EfferentCoupling = AssemblyDependencies.Count();
            NamespaceMetrics = namespaceMetrics.AsArray();
            LinesOfCode = NamespaceMetrics.Sum(x => x.LinesOfCode);
            SourceLinesOfCode = NamespaceMetrics.Sum(x => x.SourceLinesOfCode);
            MaintainabilityIndex = LinesOfCode == 0 ? 100 : NamespaceMetrics.Sum(x => x.MaintainabilityIndex * x.LinesOfCode) / LinesOfCode;
            CyclomaticComplexity = LinesOfCode == 0 ? 0 : NamespaceMetrics.Sum(x => x.CyclomaticComplexity * x.LinesOfCode) / LinesOfCode;
            Dependencies = NamespaceMetrics.SelectMany(x => x.Dependencies).Where(x => x.Assembly != Name).Distinct(Comparer).AsArray();
            Dependants = Dependencies.Select(x => x.Assembly)
                .Distinct()
                .AsArray();
            AfferentCoupling = Dependants.Count();
            var typeMetrics = NamespaceMetrics.SelectMany(x => x.TypeMetrics).AsArray();
            Abstractness = typeMetrics.Count(x => x.IsAbstract) / (double)typeMetrics.Length;
        }

        public IEnumerable<string> AssemblyDependencies { get; }

        public double Abstractness { get; }

        public int EfferentCoupling { get; }

        public int AfferentCoupling { get; }

        public int LinesOfCode { get; }

        public int SourceLinesOfCode { get; }

        public double MaintainabilityIndex { get; }

        public int CyclomaticComplexity { get; }

        public string Name { get; }

        public double RelationalCohesion { get; }

        public IEnumerable<ITypeCoupling> Dependencies { get; }

        public IEnumerable<string> Dependants { get; }

        public IEnumerable<INamespaceMetric> NamespaceMetrics { get; }
    }
}