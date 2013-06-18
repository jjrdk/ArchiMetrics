namespace ArchiTools.ScriptPack.Tests
{
	using System.Linq;
	using NUnit.Framework;
	using Roslyn.Services;

	public class ArchiToolsTests
    {
		[Test]
		public void CanStartProjectsAsSolution()
		{
			var tools = new ArchiTools();
			tools.OpenProjects(@"..\..\");
		}
    }
}
