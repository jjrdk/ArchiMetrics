namespace ArchiMeter.Raven
{
	using System;
	using Common;

	using global::Raven.Client;
	using global::Raven.Client.Document;

	public class LocalDocumentStoreProvider : IProvider<IDocumentStore>
	{
		private DocumentStore _store;

		public IDocumentStore Get()
		{
			if (_store == null)
			{
				_store = new DocumentStore
					         {
						         Url = "http://localhost:1234",
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
			}

			return _store;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~LocalDocumentStoreProvider()
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