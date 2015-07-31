// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EmptyFinalizerRuleTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the EmptyFinalizerRuleTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMetrics.CodeReview.Rules.Tests.Rules.Semantic
{
	using System.Diagnostics;
	using System.Linq;
	using System.Threading.Tasks;
	using ArchiMetrics.Analysis;
	using ArchiMetrics.CodeReview.Rules.Semantic;
	using ArchiMetrics.Common;
	using ArchiMetrics.Common.CodeReview;
	using Microsoft.CodeAnalysis;
	using NUnit.Framework;

	public sealed class EmptyFinalizerRuleTests
	{
		private EmptyFinalizerRuleTests()
		{
		}

		[TestFixture]
		public class GivenAEmptyFinalizerRule : SolutionTestsBase
		{
			private NodeReviewer _inspector;

			[SetUp]
			public void Setup()
			{
				_inspector = new NodeReviewer(new[] { new EmptyFinalizerRule(), }, Enumerable.Empty<ISymbolEvaluation>());
			}

			[TestCase(@"namespace MyNamespace
{
	public class MyClass
	{
		public MyClass()
		{
		}

		~MyClass()
		{
		}
	}
}")]
			[TestCase(@"namespace MyNamespace
{
	using System.Diagnostics;

	public class MyClass
	{
		public MyClass()
		{
		}

		~MyClass()
		{
			Debug.WriteLine(""something"");
		}
	}
}")]
			public async Task WhenClassContainsEmptyFinalizerThenReturnsError(string code)
			{
				var references = new[]
									 {
										 MetadataReference.CreateFromFile(typeof(IAvailability).Assembly.Location),
										 MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
										 MetadataReference.CreateFromFile(typeof(Debug).Assembly.Location)
									 };

				var solution = CreateSolution(references, code);
				var results = await _inspector.Inspect(solution);

				CollectionAssert.IsNotEmpty(results);
			}
		}
	}
}