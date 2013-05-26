namespace ArchiMeter.Raven.Repositories
{
	using Common;
	using Common.Documents;
	using Indexes;
	using global::Raven.Client;

	public class ComplexityMaintainabilityScatterRepository : AsyncRepositoryBase<MemberComplexityMaintainabilitySegment, MemberComplexityMaintainabilityScatterIndex>
	{
		public ComplexityMaintainabilityScatterRepository(IFactory<IAsyncDocumentSession> documentSessionFactory)
			: base(documentSessionFactory)
		{
		}
	}
}