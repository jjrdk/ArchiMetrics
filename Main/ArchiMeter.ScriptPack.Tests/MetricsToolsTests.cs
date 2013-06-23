namespace ArchiMeter.ScriptPack.Tests
{
	using System;
	using System.Linq;
	using NUnit.Framework;

	public class MetricsToolsTests
	{
		private MetricsTools _tools;

		[SetUp]
		public void Setup()
		{
			_tools = new MetricsTools();
		}

		[Test]
		public void CanCalculateMetricsForSimpleStatement()
		{
			var statement = "var x = 1;";
			var task = _tools.Calculate(statement);
			task.Wait();

			var result = task.Result;
			Console.WriteLine(result.Sum(x => x.MaintainabilityIndex));
			Assert.IsNotEmpty(result);
		}

		[Test]
		public void CanCalculateMetricsForMethod()
		{
			var statement = "public void Foo() { Console.WriteLine(\"Foo\"); }";
			var task = _tools.Calculate(statement);
			task.Wait();

			var result = task.Result;

			Assert.IsNotEmpty(result);
		}
	}
}