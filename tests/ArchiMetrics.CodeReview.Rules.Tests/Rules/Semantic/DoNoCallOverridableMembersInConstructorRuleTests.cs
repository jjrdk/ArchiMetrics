// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DoNoCallOverridableMembersInConstructorRuleTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the DoNoCallOverridableMembersInConstructorRuleTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Rules.Tests.Rules.Semantic
{
	using System.Linq;
	using System.Threading.Tasks;
	using Analysis;
	using Analysis.Common;
	using Analysis.Common.CodeReview;
	using ArchiMetrics.CodeReview.Rules.Semantic;
	using Microsoft.CodeAnalysis;
	using Xunit;

	public sealed class DoNoCallOverridableMembersInConstructorRuleTests
	{
		private DoNoCallOverridableMembersInConstructorRuleTests()
		{
		}

		public class GivenADoNoCallOverridableMembersInConstructorRule : SolutionTestsBase
		{
			private readonly NodeReviewer _inspector;

			public GivenADoNoCallOverridableMembersInConstructorRule()
			{
				_inspector = new NodeReviewer(new[] { new DoNotCallOverridableMembersInConstructorRule() }, Enumerable.Empty<ISymbolEvaluation>());
			}

            [Theory]
			[InlineData(@"namespace MyNamespace
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

			[InlineData(@"namespace MyNamespace
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
				var references = new[] { MetadataReference.CreateFromFile(typeof(IAvailability).Assembly.Location) };
				var solution = CreateSolution(references, code);
				var results = await _inspector.Inspect(solution);

				Assert.NotEmpty(results);
			}

			[InlineData(@"namespace MyNamespace
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

			[InlineData(@"namespace MyNamespace
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
				var references = new[] { MetadataReference.CreateFromFile(typeof(IAvailability).Assembly.Location) };
				var solution = CreateSolution(references, code);
				var results = await _inspector.Inspect(solution);

				Assert.Empty(results);
			}
		}
	}
}