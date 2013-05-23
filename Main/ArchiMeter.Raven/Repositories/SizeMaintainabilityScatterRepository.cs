namespace ArchiMeter.Raven.Repositories
{
	using ArchiMeter.Common;
	using ArchiMeter.Common.Documents;
	using ArchiMeter.Raven.Indexes;
	using global::Raven.Client;

	public class SizeMaintainabilityScatterRepository : AsyncRepositoryBase<MemberSizeMaintainabilitySegment, MemberSizeMaintainabilityScatterIndex>
	{
		public SizeMaintainabilityScatterRepository(IFactory<IAsyncDocumentSession> documentSessionFactory)
			: base(documentSessionFactory)
		{
		}
	}
}