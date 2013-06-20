namespace ArchiMeter.ScriptPack
{
	using ScriptCs.Contracts;

	public class IdeToolsScriptPack : IScriptPack
	{
		public void Initialize(IScriptPackSession session)
		{
			session.ImportNamespace("ArchiMeter.ScriptPack");
		}

		public IScriptPackContext GetContext()
		{
			return new IdeTools();
		}

		public void Terminate()
		{
		}
	}
}