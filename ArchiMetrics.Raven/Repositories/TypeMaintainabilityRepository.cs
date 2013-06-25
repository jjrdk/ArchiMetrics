namespace ArchiMeter.Raven.Repositories
{
	using Common;
	using Common.Documents;
	using global::Raven.Client;
	using Indexes;

	public class TypeMaintainabilityRepository : AsyncRepositoryBase<TypeMaintainabilitySegment, TfsTypeMaintainabilityDistributionIndex>
	{
		public TypeMaintainabilityRepository(IFactory<IAsyncDocumentSession> documentSessionFactory)
			: base(documentSessionFactory)
		{
		}
	}
}