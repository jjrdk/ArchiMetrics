// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RoslynExtensionTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the RoslynExtensionTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Common.Tests
{
	using System.IO;
	using NUnit.Framework;
	using Roslyn.Services;

	public class RoslynExtensionTests
	{
		[Test]
		public void CanSaveSolution()
		{
			var solution = Workspace.LoadSolution(Path.GetFullPath(@"..\..\..\ArchiMetrics.sln")).CurrentSolution;
			const string SaveLocation = @"..\..\..\x.sln";
			solution.Save(SaveLocation, true);

			var reloaded = Workspace.LoadSolution(Path.GetFullPath(SaveLocation)).CurrentSolution;

			Assert.NotNull(reloaded);
		}
	}
}
