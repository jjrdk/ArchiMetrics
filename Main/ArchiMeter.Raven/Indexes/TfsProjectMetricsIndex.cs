namespace ArchiMeter.Raven.Indexes
{
	using System.Linq;
	using Common.Documents;
	using global::Raven.Client.Indexes;

	public class TfsProjectMetricsIndex : AbstractIndexCreationTask<TfsMetricsDocument, TfsProjectMetrics>
	{
		public TfsProjectMetricsIndex()
		{
			Map = docs => from doc in docs
						  from namespaceMetric in doc.Metrics
						  from typeMetric in namespaceMetric.TypeMetrics
						  select new
									 {
										 typeMetric.CyclomaticComplexity,
										 typeMetric.LinesOfCode,
										 typeMetric.MaintainabilityIndex,
										 doc.ProjectName
									 };

			Reduce = docs => from doc in docs
							 group doc by doc.ProjectName
								 into project
								 select new
											{
												CyclomaticComplexity = project.Sum(x => x.CyclomaticComplexity),
												LinesOfCode = project.Sum(x => x.LinesOfCode),
												MaintainabilityIndex = project.Sum(x => x.MaintainabilityIndex) / project.Sum(x => 1),
												ProjectName = project.Key
											};
		}
	}
}