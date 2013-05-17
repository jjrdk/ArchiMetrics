namespace ArchiMeter.Raven.Indexes
{
	using System.Linq;

	using ArchiMeter.Common.Documents;

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
					                             CyclomaticComplexity = project.Sum(x => x.CyclomaticComplexity * x.LinesOfCode) / project.Sum(x => x.LinesOfCode),
					                             LinesOfCode = project.Sum(x => x.LinesOfCode),
					                             MaintainabilityIndex = project.Sum(x => x.MaintainabilityIndex * x.LinesOfCode) / project.Sum(x => x.LinesOfCode),
					                             ProjectName = project.Key
				                             };
		}
	}
}