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

namespace ArchiMetrics.CodeReview.Rules.Tests.Rules
{
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using Microsoft.CodeAnalysis;

	public abstract class SolutionTestsBase
	{
		protected static Solution CreateSolution(params string[] code)
		{
			return CreateSolution(
				new[]
				{
					MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
					MetadataReference.CreateFromFile(typeof(Debug).Assembly.Location)
				}, 
				code);
		}

		protected static Solution CreateSolution(IEnumerable<MetadataReference> references, params string[] code)
		{
			var workspace = new AdhocWorkspace();

			var x = 1;
			var seed = workspace.CurrentSolution.AddProject(ProjectId.CreateNewId("testcode"), "testcode", "testcode.dll", LanguageNames.CSharp);

			var projId = seed.Projects.First().Id;

			var solution = references.Aggregate(
				seed, 
				(sol, r) => sol.AddMetadataReference(projId, r));

			solution = code.Aggregate(
				solution, 
				(sol, c) => sol.AddDocument(DocumentId.CreateNewId(projId), $"TestClass{x++}.cs", c));

			return solution;
		}
	}
}