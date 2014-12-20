namespace ArchiMetrics.CodeReview.Rules.Tests.Rules.Semantic
{
	using System.Linq;
	using System.Threading.Tasks;
	using ArchiMetrics.Analysis;
	using ArchiMetrics.CodeReview.Rules.Semantic;
	using ArchiMetrics.Common;
	using ArchiMetrics.Common.CodeReview;
	using Microsoft.CodeAnalysis;
	using NUnit.Framework;

	public sealed class DoNoCallOverridableMembersInConstructorRuleTests
	{
		private DoNoCallOverridableMembersInConstructorRuleTests()
		{
		}

		public class GivenADoNoCallOverridableMembersInConstructorRule : SolutionTestsBase
		{
			private NodeReviewer _inspector;

			[SetUp]
			public void Setup()
			{
				_inspector = new NodeReviewer(new[] { new DoNotCallOverridableMembersInConstructorRule() }, Enumerable.Empty<ISymbolEvaluation>());
			}

			[TestCase(@"namespace MyNamespace
{
	public class MyClass
	{
		public MyClass()
		{
			var obj = GetItem();
		}

		private virtual object GetItem()
		{
			return new object();
		}
	}
}")]

			[TestCase(@"namespace MyNamespace
{
	public class MyClass
	{
		private object _obj;

		public MyClass()
		{
			_obj = GetItem();
		}

		private virtual object GetItem()
		{
			return new object();
		}
	}
}")]
			public async Task WhenConstructorCallsVirtualMethodThenReturnsError(string code)
			{
				var references = new[] { MetadataReference.CreateFromFile(typeof(ModelSettings).Assembly.Location) };
				var solution = CreateSolution(references, code);
				var results = await _inspector.Inspect(solution);

				CollectionAssert.IsNotEmpty(results);
			}

			[TestCase(@"namespace MyNamespace
{
	public class MyClass
	{
		public MyClass()
		{
			var obj = GetItem();
		}

		private object GetItem()
		{
			return new object();
		}
	}
}")]

			[TestCase(@"namespace MyNamespace
{
	public class MyClass
	{
		private object _obj;

		public MyClass()
		{
			_obj = GetItem();
		}

		private object GetItem()
		{
			return new object();
		}
	}
}")]
			public async Task WhenConstructorDoesNotCallVirtualMethodThenDoesNotReturnError(string code)
			{
				var references = new[] { MetadataReference.CreateFromFile(typeof(ModelSettings).Assembly.Location) };
				var solution = CreateSolution(references, code);
				var results = await _inspector.Inspect(solution);

				CollectionAssert.IsEmpty(results);
			}
		}
	}
}