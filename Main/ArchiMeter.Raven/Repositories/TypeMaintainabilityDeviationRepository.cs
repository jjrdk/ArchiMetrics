namespace ArchiMeter.Raven.Repositories
{
	using Common;
	using Common.Documents;
	using Indexes;
	using global::Raven.Client;

	public class TypeMaintainabilityDeviationRepository : AsyncRepositoryBase<TypeMaintainabilityDeviation, TfsTypeMaintainabilitySigmaIndex>
	{
		public TypeMaintainabilityDeviationRepository(IFactory<IAsyncDocumentSession> documentSessionFactory)
			: base(documentSessionFactory)
		{
		}
	}
}