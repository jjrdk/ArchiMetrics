namespace ArchiMetrics.Raven.Indexes
{
	using System.Linq;
	using Common.Documents;
	using global::Raven.Abstractions.Indexing;
	using global::Raven.Client.Indexes;

	public class TfsProjectNameIndex : AbstractIndexCreationTask<TfsMetricsDocument>
	{
		public TfsProjectNameIndex()
		{
			Map = docs => from doc in docs
			              select new
				                     {
					                     doc.ProjectName,
					                     doc.MetricsDate,
					                     doc.ProjectVersion
				                     };
			
			StoreAllFields(FieldStorage.Yes);
		}
	}
}
