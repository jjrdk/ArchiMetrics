// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RoslynTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the RoslynTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#if NCRUNCH
using NCrunch.Framework; 
#endif

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
#if NCRUNCH
			    var originalSolutionPath = NCrunchEnvironment.GetOriginalProjectPath();
                System.Diagnostics.Debug.WriteLine(originalSolutionPath);
			    var directoryName = Path.Combine(Path.GetDirectoryName(originalSolutionPath),"bin","Debug");
                System.Diagnostics.Debug.WriteLine(directoryName);
                Directory.SetCurrentDirectory(directoryName);
#endif
			var path = @"..\..\..\archimetrics.sln".GetLowerCaseFullPath();
			var workspace = MSBuildWorkspace.Create();
			var solution = await workspace.OpenSolutionAsync(path);

			Assert.True(solution.Projects.Any());
		}
	}
}
