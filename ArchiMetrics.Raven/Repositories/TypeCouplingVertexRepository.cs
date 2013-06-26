namespace ArchiMetrics.Raven.Repositories
{
	using Common;
	using Common.Structure;
	using Indexes;
	using global::Raven.Client;

	public class TypeCouplingVertexRepository : AsyncRepositoryBase<CouplingEdge, TypeCouplingVertexIndex>
	{
		public TypeCouplingVertexRepository(IFactory<IAsyncDocumentSession> documentSessionFactory)
			: base(documentSessionFactory)
		{
		}
	}
}
