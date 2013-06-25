namespace ArchiMeter.Raven.Indexes
{
	using System.Linq;
	using Common.Documents;
	using global::Raven.Client.Indexes;

	public class MemberComplexityMaintainabilityScatterIndex : AbstractIndexCreationTask<TfsMetricsDocument, MemberComplexityMaintainabilitySegment>
	{
		public MemberComplexityMaintainabilityScatterIndex()
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
					                          CyclomaticComplexity = memberMetric.CyclomaticComplexity,
					                          ProjectName = doc.ProjectName,
											  ProjectVersion = doc.ProjectVersion
				                          };

			Reduce = docs => from doc in docs
			                      group doc by new { doc.Date, doc.ProjectName, doc.ProjectVersion, doc.CyclomaticComplexity, doc.MaintainabilityIndex }
			                      into complexityMaintainability
			                      select new
				                             {
					                             MaintainabilityIndex = complexityMaintainability.Key.MaintainabilityIndex,
					                             Count = complexityMaintainability.Sum(x => x.Count),
					                             Date = complexityMaintainability.Key.Date,
					                             CyclomaticComplexity = complexityMaintainability.Key.CyclomaticComplexity,
					                             ProjectName = complexityMaintainability.Key.ProjectName,
												 ProjectVersion = complexityMaintainability.Key.ProjectVersion
				                             };
		}
	}
}