namespace ArchiMeter.ScriptPack
{
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Threading.Tasks;
	using Analysis.Metrics;
	using Common.Metrics;
	using Roslyn.Compilers;
	using Roslyn.Compilers.CSharp;
	using ScriptCs.Contracts;

	public class MetricsTools : IScriptPackContext
	{
		private readonly CodeMetricsCalculator _calculator;
		private static readonly char[] InvalidPathChars = Path.GetInvalidPathChars();

		public MetricsTools()
		{
			_calculator = new CodeMetricsCalculator();
		}

		public Task<IEnumerable<NamespaceMetric>> Calculate(params string[] snippets)
		{
			var trees = snippets.Select(GetSyntaxTree);
			var metrics = _calculator.Calculate(trees);

			return metrics;
		}

		private static SyntaxTree GetSyntaxTree(string snippet)
		{
			var path = InvalidPathChars.Intersect(snippet).Any() ? string.Empty : Path.GetFullPath(snippet);
			var parseOptions = new ParseOptions(kind: SourceCodeKind.Interactive, preprocessorSymbols: new string[0]);
			var tree = File.Exists(path)
				? SyntaxTree.ParseFile(path, parseOptions)
				: SyntaxTree.ParseText(snippet, options: parseOptions);
			return tree;
		}
	}
}