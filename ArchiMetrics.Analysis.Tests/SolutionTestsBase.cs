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
	using Microsoft.CodeAnalysis;



	public abstract class SolutionTestsBase
	{
		protected Solution CreateSolution(params string[] code)
		{
			var workspace = new CustomWorkspace(SolutionId.CreateNewId("Analysis"));

			var x = 1;
			var project = code.Aggregate(
				workspace.CurrentSolution
					.AddProject("testcode", "testcode.dll", LanguageNames.CSharp),
				(proj, c) =>
				{
					proj.AddDocument(string.Format("TestClass{0}.cs", x++), c);
					proj.AddMetadataReference(new MetadataFileReference(typeof(object).Assembly.Location));
					return proj;
				});

			return workspace.CurrentSolution;
		}
	}
}