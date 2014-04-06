namespace ArchiMetrics.CodeReview.Rules.Tests.Rules
{
	using System.Collections.Generic;
	using System.Linq;
	using Microsoft.CodeAnalysis;

	public abstract class SolutionTestsBase
	{
		protected static Solution CreateSolution(params string[] code)
		{
			return CreateSolution(
				new[]
				{
					new MetadataFileReference(typeof(object).Assembly.Location)
				},
				code);
		}

		protected static Solution CreateSolution(IEnumerable<MetadataFileReference> references, params string[] code)
		{
			var workspace = new CustomWorkspace(SolutionId.CreateNewId("Analysis"));

			var x = 1;
			var seed = workspace.CurrentSolution.AddProject(ProjectId.CreateNewId("testcode"), "testcode", "testcode.dll", LanguageNames.CSharp);

			var projId = seed.Projects.First().Id;

			var solution = references.Aggregate(
				seed,
				(sol, r) => seed.AddMetadataReference(projId, r));

			solution = code.Aggregate(
				solution,
				(sol, c) => sol.AddDocument(DocumentId.CreateNewId(projId), string.Format("TestClass{0}.cs", x++), c));

			return solution;
		}
	}
}