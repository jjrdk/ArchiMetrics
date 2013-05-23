namespace ArchiMeter.Raven.Repositories
{
	using ArchiMeter.Common;
	using ArchiMeter.Common.Documents;
	using ArchiMeter.Raven.Indexes;
	using global::Raven.Client;

	public class ComplexityMaintainabilityScatterRepository : AsyncRepositoryBase<MemberComplexityMaintainabilitySegment, MemberComplexityMaintainabilityScatterIndex>
	{
		public ComplexityMaintainabilityScatterRepository(IFactory<IAsyncDocumentSession> documentSessionFactory)
			: base(documentSessionFactory)
		{
		}
	}
}