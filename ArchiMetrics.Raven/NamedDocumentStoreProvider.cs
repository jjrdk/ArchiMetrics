namespace ArchiMetrics.Raven
{
	public class NamedDocumentStoreProvider : DocumentStoreProviderBase
	{
		private readonly string _apiKey;
		private readonly string _serverUrl;

		public NamedDocumentStoreProvider(string serverUrl, string apiKey)
		{
			_serverUrl = serverUrl;
			_apiKey = apiKey;
		}

		protected override string ServerUrl
		{
			get { return _serverUrl; }
		}

		protected override string ApiKey
		{
			get { return _apiKey; }
		}
	}
}
