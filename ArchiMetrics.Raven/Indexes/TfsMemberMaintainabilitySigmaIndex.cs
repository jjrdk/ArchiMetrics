namespace ArchiMetrics.Raven.Indexes
{
	using System;
	using System.Linq;
	using Common.Documents;
	using global::Raven.Client.Indexes;

	public class TfsMemberMaintainabilitySigmaIndex : AbstractIndexCreationTask<TfsMetricsDocument, MemberMaintainabilityDeviation>
	{
		public TfsMemberMaintainabilitySigmaIndex()
		{
			Map = docs => from doc in docs
						  from namespaceMetric in doc.Metrics
						  from typeMetric in namespaceMetric.TypeMetrics
						  from memberMetric in typeMetric.MemberMetrics
						  select new
										  {
											  ProjectName = doc.ProjectName,
											  ProjectVersion = doc.ProjectVersion,
											  NamespaceName = namespaceMetric.Name,
											  TypeName = typeMetric.Name,
											  MemberName = memberMetric.Name,
											  MaintainabilityIndex = memberMetric.MaintainabilityIndex,
											  Sigma = 0.0
										  };

			Reduce = docs => from doc in docs
							 group doc by true
								 into grouping
								 let a = new
											 {
												 AverageCC = grouping.Average(x => x.MaintainabilityIndex),
												 Items = grouping
											 }
								 let sd = new
											  {
												  AverageLoC = a.AverageCC,
												  StandardDev = Math.Sqrt(a.Items.Sum(t => (t.MaintainabilityIndex - a.AverageCC) * (t.MaintainabilityIndex - a.AverageCC)) / a.Items.Count()),
												  a.Items,
											  }
								 from t in sd.Items
								 select new
											{
												ProjectName = t.ProjectName,
												ProjectVersion = t.ProjectVersion,
												NamespaceName = t.NamespaceName,
												TypeName = t.TypeName,
												MemberName = t.MemberName,
												MaintainabilityIndex = t.MaintainabilityIndex,
												Sigma = (t.MaintainabilityIndex - a.AverageCC) / sd.StandardDev
											};
		}
	}
}
