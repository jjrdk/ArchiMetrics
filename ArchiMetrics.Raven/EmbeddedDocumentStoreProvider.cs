// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EmbeddedDocumentStoreProvider.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the EmbeddedDocumentStoreProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMeter.Raven
{
	using System;
	using Common;
	using global::Raven.Client;
	using global::Raven.Client.Embedded;

	public class EmbeddedDocumentStoreProvider : IProvider<IDocumentStore>
	{
		private EmbeddableDocumentStore _store;

		public IDocumentStore Get()
		{
			if (_store == null)
			{

				Console.WriteLine("Creating In-Memory Database");
				_store = new EmbeddableDocumentStore
							{
								RunInMemory = true,
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

		~EmbeddedDocumentStoreProvider()
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