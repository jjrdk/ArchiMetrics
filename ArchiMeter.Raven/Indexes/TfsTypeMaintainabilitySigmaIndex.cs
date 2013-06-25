namespace ArchiMeter.Raven.Indexes
{
	using System;
	using System.Linq;
	using Common.Documents;
	using global::Raven.Client.Indexes;

	public class TfsTypeMaintainabilitySigmaIndex : AbstractIndexCreationTask<TfsMetricsDocument, TypeMaintainabilityDeviation>
	{
		public TfsTypeMaintainabilitySigmaIndex()
		{
			Map = docs => from doc in docs
						  from namespaceMetric in doc.Metrics
						  from typeMetric in namespaceMetric.TypeMetrics
						  select new
									 {
										 ProjectName = doc.ProjectName,
										 ProjectVersion = doc.ProjectVersion,
										 NamespaceName = namespaceMetric.Name,
										 TypeName = typeMetric.Name,
										 MaintainabilityIndex = typeMetric.MaintainabilityIndex,
										 Sigma = 0.0
									 };

			Reduce = docs =>
					 from doc in docs
					 group doc by true
						 into grouping
						 let a = new
									 {
										 AverageMI = grouping.Average(x => x.MaintainabilityIndex),
										 Items = grouping
									 }
						 let sd = new
									  {
										  AverageMI = a.AverageMI,
										  StandardDev = Math.Sqrt(a.Items.Sum(t => (t.MaintainabilityIndex - a.AverageMI) * (t.MaintainabilityIndex - a.AverageMI)) / a.Items.Count()),
										  a.Items,
									  }
						 from t in sd.Items
						 select new
									{
										ProjectName = t.ProjectName,
										ProjectVersion = t.ProjectVersion,
										NamespaceName = t.NamespaceName,
										TypeName = t.TypeName,
										MaintainabilityIndex = t.MaintainabilityIndex,
										Sigma = (t.MaintainabilityIndex - a.AverageMI) / sd.StandardDev
									};
		}
	}
}