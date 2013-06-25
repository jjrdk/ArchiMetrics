namespace ArchiMeter.Raven
{
	using System;
	using Common;
	using global::Raven.Client;
	using global::Raven.Client.Document;
	using global::Raven.Client.Indexes;

	public abstract class DocumentStoreProviderBase : IProvider<IDocumentStore>
	{
		private DocumentStore _store;

		protected abstract string ServerUrl { get; }

		protected abstract string ApiKey { get; }

		public IDocumentStore Get()
		{
			if (_store == null)
			{
				_store = new DocumentStore
								  {
									  Url = ServerUrl,
									  ApiKey = ApiKey,
									  DefaultDatabase = "Metrics",
									  Conventions =
										  {
											  CustomizeJsonSerializer = serializer =>
												  {
													  serializer.Converters.Add(new NamespaceMetricConverter());
													  serializer.Converters.Add(new TypeMetricConverter());
													  serializer.Converters.Add(new MemberMetricConverter());
													  serializer.Converters.Add(new TypeCouplingConverter());
													  serializer.Converters.Add(new HalsteadMetricConverter());
												  }
										  }
								  };

				_store.Initialize();

				IndexCreation.CreateIndexes(GetType().Assembly, _store);
			}

			return _store;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~DocumentStoreProviderBase()
		{
			Dispose(false);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_store != null)
				{
					_store.Dispose();
				}
			}
		}
	}
}