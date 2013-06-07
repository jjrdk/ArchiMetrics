namespace ArchiMeter.CodeReview.Tests
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Text;
	using System.Xml.Linq;
	using NUnit.Framework;
	using Roslyn.Services;

	public class RoslynExtensionTests
	{
		[Test]
		public void CanSaveSolution()
		{
			var solution = Workspace.LoadSolution(Path.GetFullPath(@"..\..\..\ArchiMeter.sln")).CurrentSolution;
			solution.Save(@"..\..\..\x.sln", true);
		}

		[Test]
		public void CanMergeSolutions()
		{
			var main = Workspace.LoadSolution(Path.GetFullPath(@"..\..\..\ArchiMeter.sln")).CurrentSolution;
			var other = Workspace.LoadSolution(Path.GetFullPath(@"..\..\..\ArchiCop.sln")).CurrentSolution;
			var merged = main.MergeWith(other);

			Assert.NotNull(merged);
		}

		[Test]
		public void CanMergeSolutions2()
		{
			RoslynExtensions.MergeSolutionsTo(
				@"C:\Dev\Tfs\NewGen.Dev\Units\NewGen.sln",
				@"C:\Dev\Tfs\NewGen.Dev\Units\CWAM\CWAM.sln",
				@"C:\Dev\Tfs\NewGen.Dev\Units\IM\IM.sln",
				@"C:\Dev\Tfs\NewGen.Dev\Units\UI\UI.sln");
		}
	}

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

		private const string ProjectConfigurationFormat = @"	{{{0}}}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{{{0}}}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{{{0}}}.Release|Any CPU.ActiveCfg = Release|Any CPU
		{{{0}}}.Release|Any CPU.Build.0 = Release|Any CPU";

		public static void MergeSolutionsTo(string outputPath, params string[] solutions)
		{
			var toMerge = solutions.SelectMany(s => Workspace.LoadSolution(s).CurrentSolution.Projects);
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
			var projectIncludes = new StringBuilder();

			var projectGuids = projects.Select(p => p.FilePath).ToDictionary(s => s, GetProjectGuid);

			foreach (var project in projects.Distinct(ProjectEqualityComparer.Instance))
			{
				projectIncludes.AppendLine(string.Format(
					"Project(\"{{{3}}}\") = \"{0}\", \"{1}\", \"{{{2}}}\"",
					project.Name,
					MakeRelativePath(project.FilePath, fileName),
					projectGuids[project.FilePath],
					GetLanguageGuid(project.LanguageServices.Language)));
				projectIncludes.AppendLine("EndProject");
			}

			using (var stream = new FileStream(fileName, overwriteExisting ? FileMode.Create : FileMode.CreateNew))
			{
				using (var writer = new StreamWriter(stream))
				{
					var configs = projectGuids.Values.Select(v => string.Format(ProjectConfigurationFormat, v));
					writer.Write(string.Format(SolutionFormat, projectIncludes, string.Join(Environment.NewLine, configs)));
				}
			}
		}

		private static string GetProjectGuid(string fileName)
		{
			var xElements = XDocument.Load(fileName).Root.Descendants().ToArray();
			var guid = xElements.FirstOrDefault(e => e.Name.LocalName == "ProjectGuid").Value.Trim('{', '}');
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
				throw new ArgumentException("fromPath");
			}
			if (string.IsNullOrWhiteSpace(relativeTo))
			{
				throw new ArgumentException("relativeTo");
			}

			var fromUri = new Uri(Path.GetFullPath(fromPath));
			var toUri = new Uri(Path.GetFullPath(relativeTo));

			var relativeUri = toUri.MakeRelativeUri(fromUri);
			var relativePath = Uri.UnescapeDataString(relativeUri.ToString());

			return relativePath.Replace('/', Path.DirectorySeparatorChar);
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
			/// <param name="x">The first object of type <paramref name="T"/> to compare.</param>
			/// <param name="y">The second object of type <paramref name="T"/> to compare.</param>
			public bool Equals(IProject x, IProject y)
			{
				return x == null
						   ? y == null
						   : y != null && x.FilePath == y.FilePath;
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
				return obj == null ? 0 : obj.FilePath.GetHashCode();
			}
		}
	}
}