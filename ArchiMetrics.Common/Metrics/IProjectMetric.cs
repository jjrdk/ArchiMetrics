namespace ArchiMetrics.Common.Metrics
{
    using System.Collections.Generic;

    public interface IProjectMetric : ICodeMetric
    {
        IEnumerable<TypeCoupling> ClassCouplings { get; }

        IEnumerable<INamespaceMetric> NamespaceMetrics { get; }

        IEnumerable<string> ReferencedProjects { get; }  
    }
}