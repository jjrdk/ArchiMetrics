// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HiddenTypeDependencyRuleTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the HiddenTypeDependencyRuleTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Rules.Tests.Rules.Semantic
{
	using System.Linq;
	using System.Threading.Tasks;
	using Analysis.Common;
	using Analysis.Common.CodeReview;
	using ArchiMetrics.Analysis;
	using ArchiMetrics.CodeReview.Rules.Semantic;
	using Microsoft.CodeAnalysis;
	using Xunit;

	public sealed class HiddenTypeDependencyRuleTests
	{
		private HiddenTypeDependencyRuleTests()
		{
		}

		public class GivenAHiddenTypeDependencyRule : SolutionTestsBase
		{
			private readonly NodeReviewer _inspector;

			public GivenAHiddenTypeDependencyRule()
			{
				_inspector = new NodeReviewer(new[] { new HiddenTypeDependencyRule() }, Enumerable.Empty<ISymbolEvaluation>());
			}

            [Theory]
			[InlineData(@"namespace MyNamespace
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
			var factory = new MyFactory<ArchiMetrics.Analysis.Common.SolutionProvider>();
			return factory.Create();
		}
	}
}")]
			[InlineData(@"namespace MyNamespace
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
		private ArchiMetrics.Analysis.Common.SolutionProvider _settings = null;

		public object GetItem()
		{
			if(_settings == null)
			{
				var factory = new MyFactory<ArchiMetrics.Analysis.Common.SolutionProvider>();
				_settings = factory.Create();
			}
			return _settings;
		}
	}
}")]
			public async Task WhenMethodContainsHiddenDependencyThenReturnsError(string code)
			{
				var references = new[] { MetadataReference.CreateFromFile(typeof(IAvailability).Assembly.Location) };
				var solution = CreateSolution(references, code);
				var results = await _inspector.Inspect(solution);

				Assert.NotEmpty(results);
			}
		}
	}
}