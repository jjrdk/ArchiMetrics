namespace ArchiMetrics.Analysis.Tests
{
	using System.Linq;
	using Microsoft.CodeAnalysis;

	public abstract class SolutionTestsBase
	{
		protected Solution CreateSolution(params string[] code)
		{
			var workspace = new CustomWorkspace(SolutionId.CreateNewId("Analysis"));

			var x = 1;
			var seed = workspace.CurrentSolution.AddProject(ProjectId.CreateNewId("testcode"), "testcode", "testcode.dll", LanguageNames.CSharp);

			var projId = seed.Projects.First().Id;
			seed.AddMetadataReference(projId, new MetadataFileReference(typeof(object).Assembly.Location));
			
			var solution = code.Aggregate(
				seed,
				(sol, c) => sol.AddDocument(DocumentId.CreateNewId(projId), string.Format("TestClass{0}.cs", x++), c));

			return solution;
		}
	}
}