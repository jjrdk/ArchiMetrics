namespace ArchiMeter.Raven.Indexes
{
	using System;
	using System.Linq;
	using Common.Documents;
	using global::Raven.Client.Indexes;

	public class TfsTypeSizeComplexityGeoMeanDistributionIndex : AbstractIndexCreationTask<TfsMetricsDocument, TypeSizeComplexityGeoMeanSegment>
	{
		public TfsTypeSizeComplexityGeoMeanDistributionIndex()
		{
			Map = docs => from doc in docs
			              from namespaceMetric in doc.Metrics
			              from typeMetric in namespaceMetric.TypeMetrics
			              select new
				                     {
					                     Count = 1,
					                     GeoMean = (int)Math.Sqrt(typeMetric.LinesOfCode * typeMetric.CyclomaticComplexity),
					                     ProjectName = doc.ProjectName,
					                     Date = doc.MetricsDate
				                     };

			Reduce = docs => from doc in docs
			                 group doc by new { doc.ProjectName, doc.Date, doc.GeoMean }
			                 into sizeGroup
			                 select new
				                        {
					                        Count = sizeGroup.Sum(x => x.Count),
					                        GeoMean = sizeGroup.Key.GeoMean,
					                        ProjectName = sizeGroup.Key.ProjectName,
					                        Date = sizeGroup.Key.Date
				                        };
		}
	}
}