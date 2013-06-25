namespace ScriptCs.Metrics
{
	using ScriptCs.Contracts;

	public class MetricsScriptPack : IScriptPack
	{
		public void Initialize(IScriptPackSession session)
		{
		}

		public IScriptPackContext GetContext()
		{
			return new Metrics();
		}

		public void Terminate()
		{
		}
	}
}