namespace ArchiMetrics.Analysis.Tests.Metrics
{
	using ArchiMetrics.Analysis.Metrics;
	using NUnit.Framework;

	public class SnippetMetricsCalculatorTests
	{
		[TestCase("var x = 1;")]
		[TestCase("var t = Task.FromResult(new object());")]
		public void CanCompile(string snippet)
		{
			var calculator = new SnippetMetricsCalculator();
			var result = calculator.Calculate(snippet);

			Assert.NotNull(result);
		}
	}
}
