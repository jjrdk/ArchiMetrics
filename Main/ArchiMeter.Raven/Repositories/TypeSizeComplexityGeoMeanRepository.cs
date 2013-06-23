namespace ArchiMeter.Raven.Repositories
{
	using Common;
	using Common.Documents;
	using global::Raven.Client;
	using Indexes;

	public class TypeSizeComplexityGeoMeanRepository : AsyncRepositoryBase<TypeSizeComplexityGeoMeanSegment, TfsTypeSizeComplexityGeoMeanDistributionIndex>
	{
		public TypeSizeComplexityGeoMeanRepository(IFactory<IAsyncDocumentSession> documentSessionFactory)
			: base(documentSessionFactory)
		{
		}
	}
}