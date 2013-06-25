namespace ArchiMeter.Raven.Indexes
{
	using System.Linq;
	using Common.Documents;
	using global::Raven.Client.Indexes;

	public class TfsWeightedProjectMetricsIndex : AbstractIndexCreationTask<TfsMetricsDocument, TfsProjectMetrics>
	{
		public TfsWeightedProjectMetricsIndex()
		{
			Map = docs => from doc in docs
						  from namespaceMetric in doc.Metrics
						  from typeMetric in namespaceMetric.TypeMetrics
						  select new
									 {
										 CyclomaticComplexity = typeMetric.CyclomaticComplexity,
										 LinesOfCode = typeMetric.LinesOfCode,
										 MaintainabilityIndex = typeMetric.MaintainabilityIndex,
										 ProjectName = doc.ProjectName,
										 ProjectVersion = doc.ProjectVersion
									 };

			Reduce = docs => from doc in docs
							 group doc by new { doc.ProjectName, doc.ProjectVersion }
								 into project
								 select new
											{
												CyclomaticComplexity = project.Sum(x => x.CyclomaticComplexity * x.LinesOfCode) / project.Sum(x => x.LinesOfCode),
												LinesOfCode = project.Sum(x => x.LinesOfCode),
												MaintainabilityIndex = project.Sum(x => x.MaintainabilityIndex * x.LinesOfCode) / project.Sum(x => x.LinesOfCode),
												ProjectName = project.Key.ProjectName,
												ProjectVersion = project.Key.ProjectVersion
											};
		}
	}
}