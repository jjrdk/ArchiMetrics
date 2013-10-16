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
							new EdgeItem { Dependant = "A", Dependency = "B" }, 
							new EdgeItem { Dependant = "B", Dependency = "A" }
						};
			var task = analyzer.GetCircularReferences(items);
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
							new EdgeItem { Dependant = "A", Dependency = "B" }, 
							new EdgeItem { Dependant = "A", Dependency = "D" }, 
							new EdgeItem { Dependant = "D", Dependency = "E" }, 
							new EdgeItem { Dependant = "B", Dependency = "C" }, 
							new EdgeItem { Dependant = "C", Dependency = "A" }
						};
			var task = analyzer.GetCircularReferences(items);
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
							new EdgeItem { Dependant = "A", Dependency = "B" }, 
							new EdgeItem { Dependant = "A", Dependency = "D" }, 
							new EdgeItem { Dependant = "D", Dependency = "E" }, 
							new EdgeItem { Dependant = "B", Dependency = "C" }, 
							new EdgeItem { Dependant = "C", Dependency = "A" }, 
							new EdgeItem { Dependant = "E", Dependency = "A" }
						};
			var task = analyzer.GetCircularReferences(items);
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
			var analyzer = new DependencyAnalyzer();
			var solution = Solution.Create(SolutionId.CreateNewId("test"))
				.AddCSharpProject("sample", "sampleAssembly", out pid)
				.AddDocument(pid, "x.cs", Text, out did)
				.AddMetadataReference(pid, new MetadataFileReference(typeof(object).Assembly.Location));

			var doc = solution.GetDocument(did);
			var types = await analyzer.GetUsedTypes(doc);

			var collection = types.ToArray();
			CollectionAssert.IsNotEmpty(collection);
		}

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
			var analyzer = new DependencyAnalyzer();
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