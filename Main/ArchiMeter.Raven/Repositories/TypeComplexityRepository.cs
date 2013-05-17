namespace ArchiMeter.Raven.Repositories
{
	using ArchiMeter.Common;
	using ArchiMeter.Common.Documents;
	using ArchiMeter.Raven.Indexes;

	using global::Raven.Client;

	public class TypeComplexityRepository : AsyncRepositoryBase<TypeComplexitySegment, TfsTypeComplexityDistributionIndex>
	{
		public TypeComplexityRepository(IFactory<IAsyncDocumentSession> documentSessionFactory)
			: base(documentSessionFactory)
		{
		}
	}
}