namespace ArchiMetrics.Raven.Indexes
{
	using System.Linq;
	using Common.Documents;
	using global::Raven.Client.Indexes;

	public class MemberSizeComplexityScatterIndex : AbstractIndexCreationTask<TfsMetricsDocument, MemberSizeComplexitySegment>
	{
		public MemberSizeComplexityScatterIndex()
		{
			Map = docs => from doc in docs
						  from namespaceMetric in doc.Metrics
						  from typeMetric in namespaceMetric.TypeMetrics
						  from memberMetric in typeMetric.MemberMetrics
						  select new
									 {
										 CyclomaticComplexity = memberMetric.CyclomaticComplexity,
										 Count = 1,
										 Date = doc.MetricsDate,
										 LoC = memberMetric.LinesOfCode,
										 ProjectName = doc.ProjectName,
										 ProjectVersion = doc.ProjectVersion
									 };

			Reduce = docs => from doc in docs
			                 group doc by new { doc.Date, doc.ProjectName, doc.ProjectVersion, doc.LoC, doc.CyclomaticComplexity }
			                 into sizeComplexity
			                 select new
				                        {
					                        CyclomaticComplexity = sizeComplexity.Key.CyclomaticComplexity,
					                        Count = sizeComplexity.Sum(x => x.Count),
					                        Date = sizeComplexity.Key.Date,
					                        LoC = sizeComplexity.Key.LoC,
					                        ProjectName = sizeComplexity.Key.ProjectName,
					                        ProjectVersion = sizeComplexity.Key.ProjectVersion
				                        };
		}
	}
}
