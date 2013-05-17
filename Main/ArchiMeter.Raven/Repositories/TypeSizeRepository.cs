namespace ArchiMeter.Raven.Repositories
{
	using ArchiMeter.Common;
	using ArchiMeter.Common.Documents;
	using ArchiMeter.Raven.Indexes;

	using global::Raven.Client;

	public class TypeSizeRepository : AsyncRepositoryBase<TypeSizeSegment, TfsTypeSizeDistributionIndex>
	{
		public TypeSizeRepository(IFactory<IAsyncDocumentSession> documentSessionFactory)
			: base(documentSessionFactory)
		{
		}
	}
}