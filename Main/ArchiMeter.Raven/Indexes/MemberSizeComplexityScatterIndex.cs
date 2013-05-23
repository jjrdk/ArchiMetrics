namespace ArchiMeter.Raven.Indexes
{
	using System.Linq;
	using ArchiMeter.Common.Documents;
	using global::Raven.Client.Indexes;

	public class MemberSizeComplexityScatterIndex : AbstractIndexCreationTask<TfsMetricsDocument, MemberSizeComplexitySegment>
	{
		public MemberSizeComplexityScatterIndex()
		{
			this.Map = docs => from doc in docs
							   from namespaceMetric in doc.Metrics
							   where !namespaceMetric.Name.Contains("Tests") 
							   && !namespaceMetric.Name.Contains("UnitTest") 
							   && !namespaceMetric.Name.Contains("Fakes") 
							   && !namespaceMetric.Name.Contains("Mocks")
							   from typeMetric in namespaceMetric.TypeMetrics
							   from memberMetric in typeMetric.MemberMetrics
							   select new
										  {
											  CyclomaticComplexity = memberMetric.CyclomaticComplexity,
											  Count = 1,
											  Date = doc.MetricsDate,
											  LoC = memberMetric.LinesOfCode,
											  ProjectName = doc.ProjectName
										  };

			this.Reduce = docs => from doc in docs
								  group doc by new { doc.Date, doc.ProjectName, doc.LoC, doc.CyclomaticComplexity }
									  into sizeComplexity
									  select new
												 {
													 CyclomaticComplexity = sizeComplexity.Key.CyclomaticComplexity,
													 Count = sizeComplexity.Sum(x => x.Count),
													 Date = sizeComplexity.Key.Date,
													 LoC = sizeComplexity.Key.LoC,
													 ProjectName = sizeComplexity.Key.ProjectName
												 };
		}
	}
}