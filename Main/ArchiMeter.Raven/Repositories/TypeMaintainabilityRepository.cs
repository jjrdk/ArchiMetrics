namespace ArchiMeter.Raven.Repositories
{
	using Common;
	using Common.Documents;
	using Indexes;
	using global::Raven.Client;

	public class TypeMaintainabilityRepository : AsyncRepositoryBase<TypeMaintainabilitySegment, TfsTypeMaintainabilityDistributionIndex>
	{
		public TypeMaintainabilityRepository(IFactory<IAsyncDocumentSession> documentSessionFactory)
			: base(documentSessionFactory)
		{
		}
	}
}