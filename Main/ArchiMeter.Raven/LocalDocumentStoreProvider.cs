namespace ArchiMeter.Raven
{
	using System;

	public class LocalDocumentStoreProvider : DocumentStoreProviderBase
	{
		private readonly Uri _uri;

		public LocalDocumentStoreProvider(ushort port)
		{
			_uri = new UriBuilder(Uri.UriSchemeHttp, "localhost", port).Uri;
		}

		protected override Uri ServerUrl
		{
			get { return _uri; }
		}

		protected override string ApiKey
		{
			get { return null; }
		}
	}
}