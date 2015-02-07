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

	public abstract class SolutionTestsBase
	{
		protected Solution CreateSolution(params string[] code)
		{
			var workspace = new AdhocWorkspace();

			var x = 1;
			var seed = workspace.CurrentSolution.AddProject(ProjectId.CreateNewId("testcode"), "testcode", "testcode.dll", LanguageNames.CSharp);

			var projId = seed.Projects.First().Id;
			seed = seed.AddMetadataReference(projId, MetadataReference.CreateFromAssembly(typeof(object).Assembly));

			var solution = code.Aggregate(
				seed,
				(sol, c) => sol.AddDocument(DocumentId.CreateNewId(projId), string.Format("TestClass{0}.cs", x++), c));

			return solution;
		}
	}
}