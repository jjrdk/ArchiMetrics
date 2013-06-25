namespace ArchiMetrics.Raven.Indexes
{
	using System;
	using System.Linq;
	using Common.Documents;
	using global::Raven.Client.Indexes;

	public class TfsTypeSizeSigmaIndex : AbstractIndexCreationTask<TfsMetricsDocument, TypeSizeDeviation>
	{
		public TfsTypeSizeSigmaIndex()
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
										 LoC = typeMetric.LinesOfCode,
										 Sigma = 0.0
									 };

			Reduce = docs =>
					 from doc in docs
					 group doc by true
						 into grouping
						 let a = new
									 {
										 AverageLoC = grouping.Average(x => x.LoC),
										 Items = grouping
									 }
						 let sd = new
									  {
										  a.AverageLoC,
										  StandardDev = Math.Sqrt(a.Items.Sum(t => (t.LoC - a.AverageLoC) * (t.LoC - a.AverageLoC)) / a.Items.Count()),
										  a.Items,
									  }
						 from t in sd.Items
						 select new
									{
										ProjectName = t.ProjectName,
										ProjectVersion = t.ProjectVersion,
										NamespaceName = t.NamespaceName,
										TypeName = t.TypeName,
										LoC = t.LoC,
										Sigma = (t.LoC - a.AverageLoC) / sd.StandardDev
									};
		}
	}
}
