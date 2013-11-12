// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SlocCounter.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the SlocCounter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis
{
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Text.RegularExpressions;
	using ArchiMetrics.Common;
	using Roslyn.Compilers.Common;
	using Roslyn.Services;
	using Roslyn.Services.Formatting;

	public class SlocCounter
	{
		private static readonly Regex LinePattern = new Regex(@"^(?!(\s*\/\/))\s*.{3,}", RegexOptions.Compiled);
		private static readonly string[] Extensions = new[] { "*.cs", "*.xaml", "*.vb" };

		public int Count(string snippet)
		{
			return CountStrings(snippet.Split('\n'));
		}

		public int Count(string path, IEnumerable<string> exclusions)
		{
			return Extensions
				.SelectMany(s => Directory.GetFiles(path, s, SearchOption.AllDirectories))
				.Where(s => !exclusions.Any(e => e.ToLowerInvariant().Contains(s.ToLowerInvariant())))
				.Select(s => File.OpenText(s).ReadToEnd().Split('\n'))
				.Select(CountStrings)
				.Sum();
		}

		public int Count(IEnumerable<IProject> projects)
		{
			return projects
				.Distinct(ProjectComparer.Default)
				.SelectMany(p => p.Documents)
				.Sum(d => CountDoc(d));
		}

		public int Count(ISolution solution)
		{
			return Count(solution.Projects);
		}

		public int Count(CommonSyntaxNode node)
		{
			var lines = node.Format(FormattingOptions.GetDefaultOptions()).GetFormattedRoot().ToFullString().Split('\n');
			return CountStrings(lines);
		}

		private static int CountStrings(IEnumerable<string> strings)
		{
			return strings.Select(s => s.Trim()).Count(s => LinePattern.IsMatch(s));
		}

		private int CountDoc(IDocument document)
		{
			return Count(document.GetSyntaxTree().GetRoot());
		}
	}
}
