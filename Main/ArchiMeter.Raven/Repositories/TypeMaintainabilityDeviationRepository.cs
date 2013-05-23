namespace ArchiMeter.Raven.Repositories
{
	using ArchiMeter.Common;
	using ArchiMeter.Common.Documents;
	using ArchiMeter.Raven.Indexes;

	using global::Raven.Client;

	public class TypeMaintainabilityDeviationRepository : AsyncRepositoryBase<TypeMaintainabilityDeviation, TfsTypeMaintainabilitySigmaIndex>
	{
		public TypeMaintainabilityDeviationRepository(IFactory<IAsyncDocumentSession> documentSessionFactory)
			: base(documentSessionFactory)
		{
		}
	}
}