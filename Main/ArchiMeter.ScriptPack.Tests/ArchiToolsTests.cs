namespace ArchiMeter.ScriptPack.Tests
{
	using NUnit.Framework;

	public class ArchiToolsTests
    {
		[Test]
		public void CanStartProjectsAsSolution()
		{
			var tools = new IdeTools();
			tools.OpenProjects(@"..\..\");
		}
    }
}
