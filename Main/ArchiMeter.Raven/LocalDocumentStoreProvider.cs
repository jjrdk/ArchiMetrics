namespace ArchiMeter.Raven
{
	public class LocalDocumentStoreProvider : DocumentStoreProviderBase
	{
		private readonly ushort _port;

		public LocalDocumentStoreProvider(ushort port)
		{
			_port = port;
		}

		protected override string ServerUrl
		{
			get { return "http://localhost:" + _port; }
		}
	}
}