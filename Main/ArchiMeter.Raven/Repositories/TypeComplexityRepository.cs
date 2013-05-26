namespace ArchiMeter.Raven.Repositories
{
	using Common;
	using Common.Documents;
	using Indexes;
	using global::Raven.Client;

	public class TypeComplexityRepository : AsyncRepositoryBase<TypeComplexitySegment, TfsTypeComplexityDistributionIndex>
	{
		public TypeComplexityRepository(IFactory<IAsyncDocumentSession> documentSessionFactory)
			: base(documentSessionFactory)
		{
		}
	}
}