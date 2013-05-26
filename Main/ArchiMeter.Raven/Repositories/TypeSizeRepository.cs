namespace ArchiMeter.Raven.Repositories
{
	using Common;
	using Common.Documents;
	using Indexes;
	using global::Raven.Client;

	public class TypeSizeRepository : AsyncRepositoryBase<TypeSizeSegment, TfsTypeSizeDistributionIndex>
	{
		public TypeSizeRepository(IFactory<IAsyncDocumentSession> documentSessionFactory)
			: base(documentSessionFactory)
		{
		}
	}
}