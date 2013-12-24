// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnreadFieldRuleTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the UnreadFieldRuleTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Rules.Tests.Rules.Semantic
{
	using System.Linq;
	using ArchiMetrics.CodeReview.Rules.Semantic;
	using NUnit.Framework;
	using Roslyn.Compilers.CSharp;

	public sealed class UnreadFieldRuleTests
	{
		private UnreadFieldRuleTests()
		{
		}

		public class GivenAnUnreadFieldRule : SolutionTestsBase
		{
			private UnreadFieldRule _rule;

			[SetUp]
			public void Setup()
			{
				_rule = new UnreadFieldRule();
			}

			[TestCase(@"namespace MyNamespace
{
	public class MyClass
	{
		private object _field = new object();
	}
}")]
			[TestCase(@"namespace MyNamespace
{
	public class MyClass
	{
		private object _field;

		public MyClass()
		{
			_field = 

new object();
		}
	}
}")]
			[TestCase(@"namespace MyNamespace
{
	public class MyClass
	{
		private object _field;

		public void Something()
		{
			_field = new object();
		}
	}
}")]
			public void WhenFieldIsNeverReadThenReturnsError(string code)
			{
				var solution = CreateSolution(code);
				var classDeclaration = (from p in solution.Projects
										from d in p.Documents
										let model = d.GetSemanticModel()
										let root = d.GetSyntaxRoot()
										from n in root.DescendantNodes().OfType<FieldDeclarationSyntax>()
										select new
										{
											semanticModel = model, 
											node = n
										}).First();
				var result = _rule.Evaluate(classDeclaration.node, classDeclaration.semanticModel, solution);

				Assert.NotNull(result);
			}

			[TestCase(@"namespace MyNamespace
{
	public class MyClass
	{
		private object _field = new object();

		public void Write()
		{
			if(_field == null)
			{
				System.Console.WriteLine(""null"");
			}
		}
	}
}")]
			[TestCase(@"namespace MyNamespace
{
	public class MyClass
	{
		private object _field = new object();

		public object Get()
		{
			return _field;
		}
	}
}")]
			public void WhenFieldIsReadThenDoesNotReturnError(string code)
			{
				var solution = CreateSolution(code);
				var classDeclaration = (from p in solution.Projects
										from d in p.Documents
										let model = d.GetSemanticModel()
										let root = d.GetSyntaxRoot()
										from n in root.DescendantNodes().OfType<FieldDeclarationSyntax>()
										select new
										{
											semanticModel = model, 
											node = n
										}).First();
				var result = _rule.Evaluate(classDeclaration.node, classDeclaration.semanticModel, solution);

				Assert.Null(result);
			}
		}
	}
}