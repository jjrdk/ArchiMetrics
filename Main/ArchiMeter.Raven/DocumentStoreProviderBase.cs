namespace ArchiMeter.Raven
{
	using System;
	using ArchiMeter.Common;
	using global::Raven.Client;
	using global::Raven.Client.Document;

	public abstract class DocumentStoreProviderBase : IProvider<IDocumentStore>
	{
		private DocumentStore _store;

		protected abstract string ServerUrl { get; }

		public IDocumentStore Get()
		{
			if (_store == null)
			{
				_store = new DocumentStore
					              {
						              Url = ServerUrl,
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
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		~DocumentStoreProviderBase()
		{
			this.Dispose(false);
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