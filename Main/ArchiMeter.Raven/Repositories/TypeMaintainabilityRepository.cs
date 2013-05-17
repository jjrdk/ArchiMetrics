namespace ArchiMeter.Raven.Repositories
{
	using ArchiMeter.Common;
	using ArchiMeter.Common.Documents;
	using ArchiMeter.Raven.Indexes;

	using global::Raven.Client;

	public class TypeMaintainabilityRepository : AsyncRepositoryBase<TypeMaintainabilitySegment, TfsTypeMaintainabilityDistributionIndex>
	{
		public TypeMaintainabilityRepository(IFactory<IAsyncDocumentSession> documentSessionFactory)
			: base(documentSessionFactory)
		{
		}
	}
}