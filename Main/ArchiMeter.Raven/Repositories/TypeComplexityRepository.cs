namespace ArchiMeter.Raven.Repositories
{
	using Common;
	using Common.Documents;
	using global::Raven.Client;
	using Indexes;

	public class TypeComplexityRepository : AsyncRepositoryBase<TypeComplexitySegment, TfsTypeComplexityDistributionIndex>
	{
		public TypeComplexityRepository(IFactory<IAsyncDocumentSession> documentSessionFactory)
			: base(documentSessionFactory)
		{
		}
	}
}