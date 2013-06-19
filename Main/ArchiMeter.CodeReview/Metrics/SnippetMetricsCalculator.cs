namespace ArchiMeter.CodeReview.Metrics
{
	using Roslyn.Compilers.CSharp;

	public class SnippetMetricsCalculator
	{
		public Compilation Calculate(string snippet)
		{
			var tree = SyntaxTree.ParseText(snippet);
			var compilation = Compilation.Create("x", syntaxTrees: new[] { tree });
			return compilation;
		}
	}
}