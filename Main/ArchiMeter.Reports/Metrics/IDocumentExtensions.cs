namespace ArchiMeter.Reports.Metrics
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Text.RegularExpressions;

	using Roslyn.Services;

	public static class IDocumentExtensions
	{
		// Methods
		public static bool IsGeneratedCodeFile(this IDocument doc, IEnumerable<Regex> patterns)
		{
			var path = doc.FilePath;
			return !string.IsNullOrWhiteSpace(path) && patterns.Any(x => x.IsMatch(path));
		}
	}
}