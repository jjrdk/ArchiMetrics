namespace ArchiMeter.Raven.Repositories
{
	using Common;
	using Common.Documents;
	using global::Raven.Client;
	using Indexes;

	public class SizeMaintainabilityScatterRepository : AsyncRepositoryBase<MemberSizeMaintainabilitySegment, MemberSizeMaintainabilityScatterIndex>
	{
		public SizeMaintainabilityScatterRepository(IFactory<IAsyncDocumentSession> documentSessionFactory)
			: base(documentSessionFactory)
		{
		}
	}
}