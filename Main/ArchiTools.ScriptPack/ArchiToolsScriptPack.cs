namespace ArchiMeter.ScriptPack
{
	using ScriptCs.Contracts;

	public class ArchiToolsScriptPack : IScriptPack
	{
		public void Initialize(IScriptPackSession session)
		{
			session.AddReference("ArchiMeter.ScriptPack.dll");
			session.ImportNamespace("ArchiMeter.ScriptPack");
		}

		public IScriptPackContext GetContext()
		{
			return new ArchiTools();
		}

		public void Terminate()
		{
		}
	}
}