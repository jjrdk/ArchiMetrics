namespace ArchiMeter.Raven.Indexes
{
	using System;
	using System.Linq;

	using ArchiMeter.Common.Documents;

	using global::Raven.Client.Indexes;

	public class TfsMemberComplexitySigmaIndex : AbstractIndexCreationTask<TfsMetricsDocument, MemberComplexityDeviation>
	{
		public TfsMemberComplexitySigmaIndex()
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
					                     memberMetric.CyclomaticComplexity,
					                     Sigma = 0.0
				                     };

			Reduce = docs => from doc in docs
			                 group doc by true
			                 into grouping
			                 let a = new
				                         {
					                         AverageCC = grouping.Average(x => x.CyclomaticComplexity),
					                         Items = grouping
				                         }
			                 let sd = new
				                          {
					                          AverageLoC = a.AverageCC,
					                          StandardDev = Math.Sqrt(a.Items.Sum(t => (t.CyclomaticComplexity - a.AverageCC) * (t.CyclomaticComplexity - a.AverageCC)) / a.Items.Count()),
					                          a.Items,
				                          }
			                 from t in sd.Items
			                 select new
				                        {
					                        t.ProjectName,
					                        t.NamespaceName,
					                        t.TypeName,
					                        t.MemberName,
					                        t.CyclomaticComplexity,
					                        Sigma = (t.CyclomaticComplexity - a.AverageCC) / sd.StandardDev
				                        };
		}
	}
}