// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SolutionTestsBase.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
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
	using Roslyn.Compilers;
	using Roslyn.Services;

	public abstract class SolutionTestsBase
	{
		protected ISolution CreateSolution(params string[] code)
		{
			var x = 1;
			ProjectId pid;
			DocumentId did;
			var solution = code.Aggregate(
				Solution.Create(SolutionId.CreateNewId("Analysis"))
					.AddCSharpProject("testcode.dll", "testcode", out pid),
				(sol, c) => SolutionExtensions.AddDocument((ISolution)sol, pid, string.Format("TestClass{0}.cs", x++), (string)c, out did))
				.AddProjectReferences(pid, new ProjectId[0])
				.AddMetadataReference(pid, new MetadataFileReference(typeof(object).Assembly.Location));

			return solution;
		}
	}
}