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
			using (var workspace = new CustomWorkspace())
			{
				var x = 1;
				workspace.AddSolution(
					SolutionInfo.Create(
						SolutionId.CreateNewId("Semantic"),
						VersionStamp.Default));
				var project = code.Aggregate(
					workspace.CurrentSolution.AddProject("testcode", "testcode.dll", LanguageNames.CSharp),
					(proj, c) =>
						{
							proj.AddDocument(string.Format("TestClass{0}.cs", x++), c);
							return proj;
						})
					.AddMetadataReference(new MetadataFileReference(typeof(object).Assembly.Location));

				return workspace.CurrentSolution;
			}
		}
	}
}