namespace ArchiMeter.Raven
{
	using System;

	public class NamedDocumentStoreProvider : DocumentStoreProviderBase
	{
		private readonly string _apiKey;
		private readonly Uri _serverUrl;

		public NamedDocumentStoreProvider(Uri serverUrl, string apiKey)
		{
			_serverUrl = serverUrl;
			_apiKey = apiKey;
		}

		protected override Uri ServerUrl
		{
			get { return _serverUrl; }
		}

		protected override string ApiKey
		{
			get { return _apiKey; }
		}
	}
}