namespace ArchiMeter.Raven.Indexes
{
	using System;
	using System.Linq;

	using ArchiMeter.Common.Documents;

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
					                          doc.ProjectName,
					                          NamespaceName = namespaceMetric.Name,
					                          TypeName = typeMetric.Name,
					                          MemberName = memberMetric.Name,
					                          memberMetric.MaintainabilityIndex,
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
					                             t.ProjectName,
					                             t.NamespaceName,
					                             t.TypeName,
					                             t.MemberName,
					                             t.MaintainabilityIndex,
					                             Sigma = (t.MaintainabilityIndex - a.AverageCC) / sd.StandardDev
				                             };
		}
	}
}