namespace ArchiMeter.Raven.Repositories
{
	using Common;
	using Common.Documents;
	using Indexes;
	using global::Raven.Client;

	public class SizeMaintainabilityScatterRepository : AsyncRepositoryBase<MemberSizeMaintainabilitySegment, MemberSizeMaintainabilityScatterIndex>
	{
		public SizeMaintainabilityScatterRepository(IFactory<IAsyncDocumentSession> documentSessionFactory)
			: base(documentSessionFactory)
		{
		}
	}
}