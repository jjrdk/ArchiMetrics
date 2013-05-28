namespace ArchiMeter.Raven.Indexes
{
	using System.Linq;
	using Common.Documents;
	using global::Raven.Client.Indexes;

	public class TfsTypeMaintainabilityDistributionIndex : AbstractIndexCreationTask<TfsMetricsDocument, TypeMaintainabilitySegment>
	{
		public TfsTypeMaintainabilityDistributionIndex()
		{
			Map = docs => from doc in docs
						  from namespaceMetric in doc.Metrics
						  from typeMetric in namespaceMetric.TypeMetrics
						  select new
									 {
										 Count = 1,
										 MaintainabilityIndex = typeMetric.MaintainabilityIndex,
										 ProjectName = doc.ProjectName,
										 ProjectVersion = doc.ProjectVersion,
										 Date = doc.MetricsDate
									 };

			Reduce = docs => from doc in docs
							 group doc by new { doc.ProjectName, doc.ProjectVersion, doc.Date, doc.MaintainabilityIndex }
								 into sizeGroup
								 select new TypeMaintainabilitySegment
											{
												Count = sizeGroup.Sum(x => x.Count),
												MaintainabilityIndex = sizeGroup.Key.MaintainabilityIndex,
												ProjectName = sizeGroup.Key.ProjectName,
												ProjectVersion = sizeGroup.Key.ProjectVersion,
												Date = sizeGroup.Key.Date
											};
		}
	}
}