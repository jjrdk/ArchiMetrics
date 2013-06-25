namespace ArchiMetrics.Raven.Repositories
{
	using Common;
	using Common.Documents;
	using global::Raven.Client;
	using Indexes;

	public class SizeComplexityScatterRepository : AsyncRepositoryBase<MemberSizeComplexitySegment, MemberSizeComplexityScatterIndex>
	{
		public SizeComplexityScatterRepository(IFactory<IAsyncDocumentSession> documentSessionFactory)
			: base(documentSessionFactory)
		{
		}
	}
}
