namespace ArchiMeter.Raven.Indexes
{
	using System.Linq;

	using ArchiMeter.Common.Documents;

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

	//public class TfsProductionProjectMetricsIndex : AbstractIndexCreationTask<TfsMetricsDocument, TfsProjectMetrics>
	//{
	//	private static readonly string[] TestNames = new[]
	//									  {
	//										"Tests", 
	//										"TestInterceptor", 
	//										"Testing", 
	//										"SchedulerTest", 
	//										"Demo", 
	//										"UnitTest", 
	//										"Mock", 
	//										"DevTest", 
	//										"CodedUI", 
	//										"Fakes", 
	//										"Simulator"
	//									  };

	//	public TfsProductionProjectMetricsIndex()
	//	{
	//		Map = docs => from doc in docs
	//						   from namespaceMetric in doc.Metrics
	//						   from typeMetric in namespaceMetric.TypeMetrics
	//						   where !typeMetric.Name.Contains("Tests")
	//						   && !typeMetric.Name.Contains("TestInterceptor")
	//						   select new
	//									  {
	//										  typeMetric.CyclomaticComplexity,
	//										  typeMetric.LinesOfCode,
	//										  typeMetric.MaintainabilityIndex,
	//										  doc.ProjectName
	//									  };

	//		Reduce = docs => from doc in docs
	//							  group doc by doc.ProjectName
	//								  into project
	//								  select new
	//											 {
	//												 CyclomaticComplexity = project.Sum(x => x.CyclomaticComplexity),
	//												 LinesOfCode = project.Sum(x => x.LinesOfCode),
	//												 MaintainabilityIndex = project.Sum(x => x.MaintainabilityIndex) / project.Sum(x => 1),
	//												 ProjectName = project.Key
	//											 };
	//	}
	//}
}