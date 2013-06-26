namespace ArchiMetrics.Raven.Indexes
{
	using System.Linq;
	using Common.Documents;
	using global::Raven.Client.Indexes;

	public class TfsTypeComplexityDistributionIndex : AbstractIndexCreationTask<TfsMetricsDocument, TypeComplexitySegment>
	{
		public TfsTypeComplexityDistributionIndex()
		{
			Map = docs => from doc in docs
						  from namespaceMetric in doc.Metrics
						  from typeMetric in namespaceMetric.TypeMetrics
						  select new
									 {
										 Count = 1,
										 CyclomaticComplexity = typeMetric.CyclomaticComplexity,
										 ProjectName = doc.ProjectName,
										 ProjectVersion = doc.ProjectVersion,
										 Date = doc.MetricsDate
									 };

			Reduce = docs => from doc in docs
							 group doc by new { doc.ProjectName, doc.ProjectVersion, doc.Date, doc.CyclomaticComplexity }
								 into sizeGroup
								 select new
											{
												Count = sizeGroup.Sum(x => x.Count),
												CyclomaticComplexity = sizeGroup.Key.CyclomaticComplexity,
												ProjectName = sizeGroup.Key.ProjectName,
												ProjectVersion = sizeGroup.Key.ProjectVersion,
												Date = sizeGroup.Key.Date
											};
		}
	}
}
