namespace ArchiMetrics.Analysis.Tests
{
	using System.Linq;
	using NUnit.Framework;
	using Roslyn.Compilers;
	using Roslyn.Services;

	public class ControlFlowAnalyzerTests
	{
		[Test]
		public void CanAnalyzeControlFlow()
		{
			const string Text = @"namespace abc
{
	using System;

	public class MyClass
	{
		public int Write(int x)
		{
			if(x % 2 == 0)
			{
				Console.WriteLine(""Hello World"");
				return x + 1;
			}
			else
			{
				return x;
			}
		}
	}
}";
			DocumentId did;
			ProjectId pid;
			var analyzer = new FlowAnalyzer();
			var solution = Solution.Create(SolutionId.CreateNewId("test"))
				.AddCSharpProject("sample", "sampleAssembly", out pid)
				.AddDocument(pid, "x.cs", Text, out did)
				.AddMetadataReference(pid, new MetadataFileReference(typeof(object).Assembly.Location));

			var doc = solution.GetDocument(did);
			var flows = analyzer.GetControlFlows(doc);

			CollectionAssert.IsNotEmpty(flows.Result.ToArray());
		}
	}
}