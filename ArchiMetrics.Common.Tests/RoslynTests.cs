namespace ArchiMetrics.Common.Tests
{
	using System.IO;
	using System.Linq;
	using System.Threading.Tasks;
	using Microsoft.CodeAnalysis.MSBuild;
	using NUnit.Framework;

	public class RoslynTests
	{
		[Test]
		public async Task WhenLoadingSolutionThenHasProjects()
		{
			var path = Path.GetFullPath(@"..\..\..\archimetrics.sln");
			var workspace = MSBuildWorkspace.Create();
			var solution = await workspace.OpenSolutionAsync(path);

			Assert.True(solution.Projects.Any());
		}
	}
}
