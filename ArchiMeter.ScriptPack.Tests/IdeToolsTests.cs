namespace ArchiMeter.ScriptPack.Tests
{
	using NUnit.Framework;
	using ScriptCs.Metrics;

	public class IdeToolsTests
	{
		[Test]
		public void CanStartProjectsAsSolution()
		{
			var tools = new IdeTools();
			tools.OpenProjects(@"..\..\");
		}
	}
}
