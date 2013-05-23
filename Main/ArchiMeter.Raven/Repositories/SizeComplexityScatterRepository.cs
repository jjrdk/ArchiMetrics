namespace ArchiMeter.Raven.Repositories
{
	using ArchiMeter.Common;
	using ArchiMeter.Common.Documents;
	using ArchiMeter.Raven.Indexes;
	using global::Raven.Client;

	public class SizeComplexityScatterRepository : AsyncRepositoryBase<MemberSizeComplexitySegment, MemberSizeComplexityScatterIndex>
	{
		public SizeComplexityScatterRepository(IFactory<IAsyncDocumentSession> documentSessionFactory)
			: base(documentSessionFactory)
		{
		}
	}
}