namespace ArchiMetrics.Analysis.Metrics
{
    using System.Linq;
    using System.Collections.Generic;
    using ArchiMetrics.Common.Metrics;

    internal class ProjectMetric : IProjectMetric
    {
        private static readonly IEqualityComparer<TypeCoupling> Comparer = new ComparableComparer<TypeCoupling>();

        public ProjectMetric(string name, IEnumerable<INamespaceMetric> namespaceMetrics, IEnumerable<string> referencedProjects)
        {
            Name = name;
            ReferencedProjects = referencedProjects.ToArray();
            NamespaceMetrics = namespaceMetrics.ToArray();
            LinesOfCode = NamespaceMetrics.Sum(x => x.LinesOfCode);
            MaintainabilityIndex = NamespaceMetrics.Sum(x => x.MaintainabilityIndex * x.LinesOfCode) / LinesOfCode;
            CyclomaticComplexity = NamespaceMetrics.Sum(x => x.CyclomaticComplexity * x.LinesOfCode) / LinesOfCode;
            ClassCouplings = NamespaceMetrics.SelectMany(x => x.ClassCouplings).Distinct(Comparer).ToArray();
        }

        public int LinesOfCode { get; private set; }

        public double MaintainabilityIndex { get; private set; }

        public int CyclomaticComplexity { get; private set; }

        public string Name { get; private set; }

        public IEnumerable<string> ReferencedProjects { get; set; }

        public IEnumerable<INamespaceMetric> NamespaceMetrics { get; private set; }

        public IEnumerable<TypeCoupling> ClassCouplings { get; private set; }
    }
}