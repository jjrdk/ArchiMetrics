namespace ArchiMetrics.CodeReview.Rules.Tests.Rules.Semantic
{
	using System.Threading.Tasks;
	using ArchiMetrics.Analysis;
	using ArchiMetrics.CodeReview.Rules.Semantic;
	using ArchiMetrics.Common;
	using Microsoft.CodeAnalysis;
	using NUnit.Framework;

	public sealed class HiddenTypeDependencyRuleTests
	{
		private HiddenTypeDependencyRuleTests()
		{
		}

		public class GivenAHiddenTypeDependencyRule : SolutionTestsBase
		{
			private NodeReviewer _inspector;

			[SetUp]
			public void Setup()
			{
				_inspector = new NodeReviewer(new[] { new HiddenTypeDependencyRule() });
			}

			[TestCase(@"namespace MyNamespace
{
	public class MyFactory<T> where T : new()
{
	public T Create()
	{
		return new T();
	}
}

	public class MyClass
	{
		public object GetItem()
		{
			var factory = new Factory<ArchiMetrics.Common.ModelSettings>();
			return factory.Create();
		}
	}
}")]
			[TestCase(@"namespace MyNamespace
{
	public class MyFactory<T> where T : new()
{
	public T Create()
	{
		return new T();
	}
}

	public class MyClass
	{
		private ArchiMetrics.Common.ModelSettings _settings = null;

		public object GetItem()
		{
			if(_settings == null)
			{
				var factory = new Factory<ArchiMetrics.Common.ModelSettings>();
				_settings = factory.Create();
			}
			return _settings;
		}
	}
}")]
			public async Task WhenMethodContainsHiddenDependencyThenReturnsError(string code)
			{
				var references = new[] { new MetadataFileReference(typeof(ModelSettings).Assembly.Location) };
				var solution = CreateSolution(references, code);
				var results = await _inspector.Inspect(solution);

				CollectionAssert.IsNotEmpty(results);
			}
		}
	}
}