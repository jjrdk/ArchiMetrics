namespace ArchiMeter.Raven.Indexes
{
	using System.Linq;
	using ArchiMeter.Common.Documents;
	using global::Raven.Abstractions.Indexing;
	using global::Raven.Client.Indexes;

	public class TfsProjectNameIndex : AbstractIndexCreationTask<TfsMetricsDocument>
	{
		public TfsProjectNameIndex()
		{
			this.Map = docs => from doc in docs
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