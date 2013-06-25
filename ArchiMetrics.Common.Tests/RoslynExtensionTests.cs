namespace ArchiMetrics.Common.Tests
{
	using System.IO;
	using NUnit.Framework;
	using Roslyn.Services;

	public class RoslynExtensionTests
	{
		[Test]
		public void CanSaveSolution()
		{
			var solution = Workspace.LoadSolution(Path.GetFullPath(@"..\..\..\ArchiMetrics.sln")).CurrentSolution;
			const string SaveLocation = @"..\..\..\x.sln";
			solution.Save(SaveLocation, true);

			var reloaded = Workspace.LoadSolution(Path.GetFullPath(SaveLocation)).CurrentSolution;

			Assert.NotNull(reloaded);
		}
	}
}
