namespace ArchiMeter.Raven
{
	using System;
	using Common;
	using global::Raven.Client;
	using global::Raven.Client.Indexes;

	public class SessionFactory : IFactory<IDocumentSession>
	{
		private readonly IDocumentStore _store;

		public SessionFactory(IProvider<IDocumentStore> documentStoreProvider)
		{
			_store = documentStoreProvider.Get();

			IndexCreation.CreateIndexes(GetType().Assembly, _store);
		}

		public IDocumentSession Create()
		{
			return _store.OpenSession();
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~SessionFactory()
		{
			Dispose(false);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				// _store.Dispose();
			}
		}
	}
}