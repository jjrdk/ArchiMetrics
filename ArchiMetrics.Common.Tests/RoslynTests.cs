// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RoslynTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the RoslynTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

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
			var path = @"..\..\..\archimetrics.sln".GetLowerCaseFullPath();
			var workspace = MSBuildWorkspace.Create();
			var solution = await workspace.OpenSolutionAsync(path);

			Assert.True(solution.Projects.Any());
		}
	}
}
