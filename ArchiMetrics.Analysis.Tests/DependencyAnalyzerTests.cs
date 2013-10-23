// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DependencyAnalyzerTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the DependencyAnalyzerTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Tests
{
	using System.Linq;
	using ArchiMetrics.Common.Structure;
	using NUnit.Framework;
	using Roslyn.Compilers;
	using Roslyn.Services;
	using Roslyn.Services.CSharp;

	public class DependencyAnalyzerTests
	{
		[Test]
		public void CanCreateDirectDepencyChain()
		{
			var analyzer = new DependencyAnalyzer();
			var items = new[]
						{
							new MetricsEdgeItem { Dependant = "A", Dependency = "B" }, 
							new MetricsEdgeItem { Dependant = "B", Dependency = "A" }
						};
			var task = DependencyAnalyzer.GetCircularReferences(items);
			task.Wait();
			var chains = task.Result.ToArray();

			Assert.AreEqual(1, chains.Length);
		}

		[Test]
		public void CanCreateNonDirectDepencyChain()
		{
			var analyzer = new DependencyAnalyzer();
			var items = new[]
						{
							new MetricsEdgeItem { Dependant = "A", Dependency = "B" }, 
							new MetricsEdgeItem { Dependant = "A", Dependency = "D" }, 
							new MetricsEdgeItem { Dependant = "D", Dependency = "E" }, 
							new MetricsEdgeItem { Dependant = "B", Dependency = "C" }, 
							new MetricsEdgeItem { Dependant = "C", Dependency = "A" }
						};
			var task = DependencyAnalyzer.GetCircularReferences(items);
			task.Wait();
			var chains = task.Result.ToArray();

			Assert.AreEqual(1, chains.Length);
		}

		[Test]
		public void CanCreateMultipleNonDirectDepencyChain()
		{
			var analyzer = new DependencyAnalyzer();
			var items = new[]
						{
							new MetricsEdgeItem { Dependant = "A", Dependency = "B" }, 
							new MetricsEdgeItem { Dependant = "A", Dependency = "D" }, 
							new MetricsEdgeItem { Dependant = "D", Dependency = "E" }, 
							new MetricsEdgeItem { Dependant = "B", Dependency = "C" }, 
							new MetricsEdgeItem { Dependant = "C", Dependency = "A" }, 
							new MetricsEdgeItem { Dependant = "E", Dependency = "A" }
						};
			var task = DependencyAnalyzer.GetCircularReferences(items);
			task.Wait();
			var chains = task.Result.ToArray();

			Assert.AreEqual(2, chains.Length);
		}

		[Test]
		public async void CanFindTypesInDocument()
		{
			const string Text = @"namespace abc
{
	public class MyClass
	{
		public void Write()
		{
			System.Console.WriteLine(""Hello World"");
		}
	}
}";
			DocumentId did;
			ProjectId pid;
			var solution = Solution.Create(SolutionId.CreateNewId("test"))
				.AddCSharpProject("sample", "sampleAssembly", out pid)
				.AddDocument(pid, "x.cs", Text, out did)
				.AddMetadataReference(pid, new MetadataFileReference(typeof(object).Assembly.Location));

			var doc = solution.GetDocument(did);
            var types = await DependencyAnalyzer.GetUsedTypes(doc);

			var collection = types.ToArray();
			CollectionAssert.IsNotEmpty(collection);
		}
	}
}