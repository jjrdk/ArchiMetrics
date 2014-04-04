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

namespace ArchiMetrics.CodeReview.Rules.Tests.Rules
{
	using System.Collections.Generic;
	using System.Linq;
	using Microsoft.CodeAnalysis;

	public abstract class SolutionTestsBase
	{
		protected Solution CreateSolution(params string[] code)
		{
			return CreateSolution(Enumerable.Empty<MetadataFileReference>(), code);
		}

		protected Solution CreateSolution(IEnumerable<MetadataFileReference> references, params string[] code)
		{
			var x = 1;
			ProjectId pid;
			DocumentId did;
			var solution = code.Aggregate(
				Solution.Create(SolutionId.CreateNewId("Semantic"))
					.AddCSharpProject("testcode.dll", "testcode", out pid),
				(sol, c) => sol.AddDocument(pid, string.Format("TestClass{0}.cs", x++), c, out did))
				.AddProjectReferences(pid, new ProjectId[0])
				.AddMetadataReference(pid, new MetadataFileReference(typeof(object).Assembly.Location));

			solution = solution.AddMetadataReferences(pid, references);

			return solution;
		}
	}
}