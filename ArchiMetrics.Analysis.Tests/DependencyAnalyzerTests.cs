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

	using Common;

	using NUnit.Framework;

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
	}
}