namespace ArchiMetrics.Analysis.Tests
{
	using Microsoft.CodeAnalysis.CSharp;
	using NUnit.Framework;

	public sealed class CodeMetricsCalculatorTests
	{
		private CodeMetricsCalculatorTests()
		{
		}

		public class GivenACodeMetricsCalculator
		{
			private CodeMetricsCalculator _calculator;

			[SetUp]
			public void Setup()
			{
				_calculator = new CodeMetricsCalculator();
			}

			[Test]
			public void WhenCalculatingMetricsForCodeSnippetThenReturnsMetrics()
			{
				var snippet = @"public int GetValue(int x)
{
	if(x% 2 == 0)
	{
		return x-2;
	}

	return x;
}";
				var tree = CSharpSyntaxTree.ParseText(snippet);
				var metrics = _calculator.Calculate(new[] { tree });

				Assert.NotNull(metrics);
			}
		}
	}
}
