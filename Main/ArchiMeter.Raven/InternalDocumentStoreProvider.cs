namespace ArchiMeter.Raven
{
	public class NamedDocumentStoreProvider : DocumentStoreProviderBase
	{
		private readonly string _serverUrl;

		public NamedDocumentStoreProvider(string serverUrl)
		{
			_serverUrl = serverUrl;
		}

		protected override string ServerUrl
		{
			get { return _serverUrl; }
		}
	}
}