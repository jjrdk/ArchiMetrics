namespace ArchiMeter.Raven.Repositories
{
	using Common;
	using Common.Documents;
	using global::Raven.Client;
	using Indexes;

	public class TypeSizeRepository : AsyncRepositoryBase<TypeSizeSegment, TfsTypeSizeDistributionIndex>
	{
		public TypeSizeRepository(IFactory<IAsyncDocumentSession> documentSessionFactory)
			: base(documentSessionFactory)
		{
		}
	}
}