// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RoslynExtensions.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the RoslynExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Common
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Xml.Linq;
	using Roslyn.Services;

	public static class RoslynExtensions
	{
		private const string SolutionFormat = @"
Microsoft Visual Studio Solution File, Format Version 12.00
# Visual Studio 2012
{0}
Global
	GlobalSection(SolutionConfigurationPlatforms) = preSolution
		Debug|Any CPU = Debug|Any CPU
		Release|Any CPU = Release|Any CPU
	EndGlobalSection
	GlobalSection(SolutionConfiguration) = preSolution
		ConfigName.0 = Debug
		ConfigName.1 = Release
	EndGlobalSection
	GlobalSection(ProjectConfigurationPlatforms) = postSolution
		{1}
	EndGlobalSection
EndGlobal
";

		private const string ProjectConfigurationFormat = @"		{{{0}}}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{{{0}}}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{{{0}}}.Release|Any CPU.ActiveCfg = Release|Any CPU
		{{{0}}}.Release|Any CPU.Build.0 = Release|Any CPU";

		public static void MergeSolutionsTo(this string outputPath, params string[] solutions)
		{
			var toMerge = solutions
				.SelectMany(s => Workspace.LoadSolution(s).CurrentSolution.Projects)
				.Select(p => p.FilePath)
				.Distinct()
				.ToArray();
			MergeProjectsTo(outputPath, toMerge);
		}

		public static void MergeProjectsTo(this string outputPath, params string[] projects)
		{
			var distinct = projects.Distinct().ToArray();
			var projectsAndReferences = distinct
				.Concat(distinct.SelectMany(GetReferencePaths))
				.Distinct()
				.ToArray();
			var toMerge = projectsAndReferences.Select(s =>
										  {
											  try
											  {
												  var standAloneProject = Workspace.LoadStandAloneProject(s);
												  var p = standAloneProject.CurrentSolution.Projects;
												  return p.LastOrDefault();
											  }
											  catch (Exception e)
											  {
												  Console.WriteLine(e.Message);
												  return null;
											  }
										  })
										  .Where(p => p != null)
										  .ToArray();

			WriteProjects(toMerge, outputPath, true);
		}

		public static ISolution MergeWith(this ISolution main, params ISolution[] others)
		{
			var allProjects = main.Projects.Concat(others.SelectMany(s => s.Projects));
			var mergedFile = main.FilePath.Replace(".sln", "-merged.sln");
			WriteProjects(allProjects, mergedFile, true);

			return Workspace.LoadSolution(mergedFile).CurrentSolution;
		}

		public static void Save(this ISolution solution, string fileName, bool overwriteExisting)
		{
			if (!overwriteExisting && File.Exists(fileName))
			{
				return;
			}

			WriteProjects(solution.Projects, fileName, overwriteExisting);
		}

		private static void WriteProjects(IEnumerable<IProject> projects, string fileName, bool overwriteExisting)
		{
			var distinctProjects = projects
				.GroupBy(p => p.FilePath)
				.Select(g => g.First())
				.ToArray();
			var projectGuids = distinctProjects
				.Select(p => p.FilePath)
				.Distinct()
				.Where(File.Exists)
				.ToDictionary(s => s, GetProjectGuid);

			var projectIncludes = string.Join(
				Environment.NewLine,
				distinctProjects
				.Select(project => string.Format(
					"Project(\"{{{0}}}\") = \"{1}\", \"{2}\", \"{{{3}}}\"{4}EndProject{4}",
					GetLanguageGuid(project.LanguageServices.Language),
					project.Name,
					MakeRelativePath(project.FilePath, fileName),
					projectGuids[project.FilePath],
					Environment.NewLine).Trim()));

			using (var stream = new FileStream(fileName, overwriteExisting ? FileMode.Create : FileMode.CreateNew))
			{
				var writer = new StreamWriter(stream);
				var configs = projectGuids.Values.Select(v => string.Format(ProjectConfigurationFormat, v));
				writer.Write(
					SolutionFormat,
					projectIncludes.Trim(),
					string.Join(Environment.NewLine, configs).Trim());
				writer.Flush();
				writer.Close();
			}
		}

		private static string GetProjectGuid(string fileName)
		{
			var root = XDocument.Load(fileName).Root;
			var ns = root.GetDefaultNamespace();
			var guid = root
				.Descendants(ns + "ProjectGuid")
				.First()
				.Value.Trim('{', '}');
			return guid;
		}

		private static string GetLanguageGuid(string language)
		{
			switch (language)
			{
				case "C#":
					return "FAE04EC0-301F-11D3-BF4B-00C04F79EFBC";
				case "VB.NET":
					return "F184B08F-C81C-45F6-A57F-5ABD9991F28F";
				default:
					return string.Empty;
			}
		}

		private static string MakeRelativePath(string fromPath, string relativeTo)
		{
			if (string.IsNullOrWhiteSpace(fromPath))
			{
				throw new ArgumentException("Expected actual path", "fromPath");
			}

			if (string.IsNullOrWhiteSpace(relativeTo))
			{
				throw new ArgumentException("Expected actual path", "relativeTo");
			}

			var fromUri = new Uri(Path.GetFullPath(fromPath));
			var toUri = new Uri(Path.GetFullPath(relativeTo));

			var relativeUri = toUri.MakeRelativeUri(fromUri);
			var relativePath = Uri.UnescapeDataString(relativeUri.ToString());

			return relativePath.Replace('/', Path.DirectorySeparatorChar);
		}

		private static string[] GetReferencePaths(string fileName)
		{
			if (!File.Exists(fileName))
			{
				return new string[0];
			}

			var root = XDocument.Load(fileName).Root;
			var ns = root.GetDefaultNamespace();
			var references = root.Descendants(XName.Get("ProjectReference", ns.NamespaceName)).ToArray();
			var paths = references
				.Select(x => x.Attribute("Include").Value)
				.Select(x => Path.GetFullPath(Path.Combine(Path.GetDirectoryName(fileName), x)))
				.ToArray();
			var childReferences = paths.SelectMany(GetReferencePaths);
			return paths.Concat(childReferences).Distinct().ToArray();
		}

		private class ProjectEqualityComparer : IEqualityComparer<IProject>
		{
			private static readonly ProjectEqualityComparer Comparer = new ProjectEqualityComparer();

			private ProjectEqualityComparer()
			{
			}

			public static ProjectEqualityComparer Instance
			{
				get { return Comparer; }
			}

			/// <summary>
			/// Determines whether the specified objects are equal.
			/// </summary>
			/// <returns>
			/// True if the specified objects are equal; otherwise, false.
			/// </returns>
			/// <param name="x">The first object of type IProject to compare.</param>
			/// <param name="y">The second object of type IProject to compare.</param>
			public bool Equals(IProject x, IProject y)
			{
				var result = x == null
						   ? y == null
						   : y != null && x.Name == y.Name;

				return result;
			}

			/// <summary>
			/// Returns a hash code for the specified object.
			/// </summary>
			/// <returns>
			/// A hash code for the specified object.
			/// </returns>
			/// <param name="obj">The <see cref="T:System.Object"/> for which a hash code is to be returned.</param><exception cref="T:System.ArgumentNullException">The type of <paramref name="obj"/> is a reference type and <paramref name="obj"/> is null.</exception>
			public int GetHashCode(IProject obj)
			{
				return obj == null ? 0 : obj.Name.GetHashCode();
			}
		}
	}
}
