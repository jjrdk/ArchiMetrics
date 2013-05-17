namespace ArchiMeter.Raven.Indexes
{
	using System.Linq;
	using ArchiMeter.Common.Documents;
	using global::Raven.Client.Indexes;

	public class TfsTypeSizeDistributionIndex : AbstractIndexCreationTask<TfsMetricsDocument, TypeSizeSegment>
	{
		public TfsTypeSizeDistributionIndex()
		{
			Map = docs => from doc in docs
						  from namespaceMetric in doc.Metrics
						  from typeMetric in namespaceMetric.TypeMetrics
						  select new
									 {
										 Count = 1,
										 LoC = typeMetric.LinesOfCode,
										 ProjectName = doc.ProjectName,
										 Date = doc.MetricsDate
									 };

			Reduce = docs => from doc in docs
							 group doc by new { doc.ProjectName, doc.Date, doc.LoC }
								 into sizeGroup
								 select new
											{
												Count = sizeGroup.Sum(x => x.Count),
												LoC = sizeGroup.Key.LoC,
												ProjectName = sizeGroup.Key.ProjectName,
												Date = sizeGroup.Key.Date
											};
		}
	}
}