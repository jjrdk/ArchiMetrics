namespace ArchiMeter.Raven.Repositories
{
	using Common;
	using Common.Documents;
	using Indexes;
	using global::Raven.Client;

	public class SizeComplexityScatterRepository : AsyncRepositoryBase<MemberSizeComplexitySegment, MemberSizeComplexityScatterIndex>
	{
		public SizeComplexityScatterRepository(IFactory<IAsyncDocumentSession> documentSessionFactory)
			: base(documentSessionFactory)
		{
		}
	}
}