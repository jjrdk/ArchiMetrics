namespace ArchiMetrics.Raven
{
	public class LocalDocumentStoreProvider : DocumentStoreProviderBase
	{
		private readonly string _uri;

		public LocalDocumentStoreProvider(ushort port)
		{
			_uri = "http://localhost:" + port;
		}

		protected override string ServerUrl
		{
			get { return _uri; }
		}

		protected override string ApiKey
		{
			get { return null; }
		}
	}
}
