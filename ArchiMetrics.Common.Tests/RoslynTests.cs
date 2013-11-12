// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RoslynTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
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
	using NUnit.Framework;
	using Roslyn.Services;

	public class RoslynTests
	{
		[Test]
		public void WhenLoadingSolutionThenHasProjects()
		{
			var path = Path.GetFullPath(@"..\..\..\archimetrics.sln");
			var workspace = Workspace.LoadSolution(path, "Release");
			var solution = workspace.CurrentSolution;
		
			Assert.True(solution.HasProjects);
		}
	}
}
