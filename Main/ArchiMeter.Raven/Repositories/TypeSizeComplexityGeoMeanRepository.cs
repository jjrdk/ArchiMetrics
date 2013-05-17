namespace ArchiMeter.Raven.Repositories
{
	using ArchiMeter.Common;
	using ArchiMeter.Common.Documents;
	using ArchiMeter.Raven.Indexes;

	using global::Raven.Client;

	public class TypeSizeComplexityGeoMeanRepository : AsyncRepositoryBase<TypeSizeComplexityGeoMeanSegment, TfsTypeSizeComplexityGeoMeanDistributionIndex>
	{
		public TypeSizeComplexityGeoMeanRepository(IFactory<IAsyncDocumentSession> documentSessionFactory)
			: base(documentSessionFactory)
		{
		}
	}
}