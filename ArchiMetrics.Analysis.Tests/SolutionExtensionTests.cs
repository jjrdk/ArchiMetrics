// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SolutionExtensionTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the SolutionExtensionTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Tests
{
	using System.IO;
	using System.Threading.Tasks;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.MSBuild;
	using NUnit.Framework;


	public class SolutionExtensionTests
	{
		[Test]
		public async Task CanSaveSolution()
		{
			var workspace = MSBuildWorkspace.Create();
			var solution = await workspace.OpenSolutionAsync(Path.GetFullPath(@"..\..\..\ArchiMetrics.sln"));
			const string SaveLocation = @"..\..\..\x.sln";
			solution.Save(SaveLocation, true);

			var reloaded = await workspace.OpenSolutionAsync(Path.GetFullPath(SaveLocation));

			Assert.NotNull(reloaded);
		}
	}
}
