namespace ArchiMeter.ScriptPack
{
	using ScriptCs.Contracts;

	public class MetricsToolsScriptPack : IScriptPack
	{
		public void Initialize(IScriptPackSession session)
		{
			session.ImportNamespace("ArchiMeter.ScriptPack");
		}

		public IScriptPackContext GetContext()
		{
			return new MetricsTools();
		}

		public void Terminate()
		{
		}
	}
}