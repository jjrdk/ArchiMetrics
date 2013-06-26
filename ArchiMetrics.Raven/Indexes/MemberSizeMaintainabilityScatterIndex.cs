namespace ArchiMetrics.Raven.Indexes
{
	using System.Linq;
	using Common.Documents;
	using global::Raven.Client.Indexes;

	public class MemberSizeMaintainabilityScatterIndex : AbstractIndexCreationTask<TfsMetricsDocument, MemberSizeMaintainabilitySegment>
	{
		public MemberSizeMaintainabilityScatterIndex()
		{
			Map = docs => from doc in docs
			              from namespaceMetric in doc.Metrics
			              from typeMetric in namespaceMetric.TypeMetrics
			              from memberMetric in typeMetric.MemberMetrics
			              select new
				                     {
					                     MaintainabilityIndex = memberMetric.MaintainabilityIndex,
					                     Count = 1,
					                     Date = doc.MetricsDate,
					                     LoC = memberMetric.LinesOfCode,
					                     ProjectName = doc.ProjectName,
										 ProjectVersion = doc.ProjectVersion
				                     };

			Reduce = docs => from doc in docs
			                 group doc by new { doc.Date, doc.ProjectName, doc.ProjectVersion, doc.LoC, doc.MaintainabilityIndex }
			                 into sizeComplexity
			                 select new
				                        {
					                        MaintainabilityIndex = sizeComplexity.Key.MaintainabilityIndex,
					                        Count = sizeComplexity.Sum(x => x.Count),
					                        Date = sizeComplexity.Key.Date,
					                        LoC = sizeComplexity.Key.LoC,
					                        ProjectName = sizeComplexity.Key.ProjectName,
											ProjectVersion = sizeComplexity.Key.ProjectVersion
				                        };
		}
	}
}
