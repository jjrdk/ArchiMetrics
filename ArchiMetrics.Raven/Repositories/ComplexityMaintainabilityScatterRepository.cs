namespace ArchiMeter.Raven.Repositories
{
	using Common;
	using Common.Documents;
	using global::Raven.Client;
	using Indexes;

	public class ComplexityMaintainabilityScatterRepository : AsyncRepositoryBase<MemberComplexityMaintainabilitySegment, MemberComplexityMaintainabilityScatterIndex>
	{
		public ComplexityMaintainabilityScatterRepository(IFactory<IAsyncDocumentSession> documentSessionFactory)
			: base(documentSessionFactory)
		{
		}
	}
}