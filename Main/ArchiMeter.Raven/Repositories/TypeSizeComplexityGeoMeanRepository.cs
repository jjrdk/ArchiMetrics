namespace ArchiMeter.Raven.Repositories
{
	using Common;
	using Common.Documents;
	using Indexes;
	using global::Raven.Client;

	public class TypeSizeComplexityGeoMeanRepository : AsyncRepositoryBase<TypeSizeComplexityGeoMeanSegment, TfsTypeSizeComplexityGeoMeanDistributionIndex>
	{
		public TypeSizeComplexityGeoMeanRepository(IFactory<IAsyncDocumentSession> documentSessionFactory)
			: base(documentSessionFactory)
		{
		}
	}
}