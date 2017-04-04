// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SolutionTestsBase.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the SolutionTestsBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Tests
{
	using System.Linq;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.MSBuild;

    public abstract class SolutionTestsBase
	{
		protected Solution CreateSolution(params string[] code)
		{
			var workspace = MSBuildWorkspace.Create();// AdhocWorkspace();

			var x = 1;
			var seed = workspace.CurrentSolution.AddProject(ProjectId.CreateNewId("testcode"), "testcode", "testcode.dll", LanguageNames.CSharp);

			var projId = seed.Projects.First().Id;
			seed = seed.AddMetadataReference(projId, MetadataReference.CreateFromFile(typeof(object).Assembly.Location));

			var solution = code.Aggregate(
				seed,
				(sol, c) => sol.AddDocument(DocumentId.CreateNewId(projId), $"TestClass{x++}.cs", c));

			return solution;
		}
	}
}