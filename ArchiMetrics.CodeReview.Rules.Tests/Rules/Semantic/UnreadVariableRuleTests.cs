// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnreadVariableRuleTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the UnreadVariableRuleTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Rules.Tests.Rules.Semantic
{
	using System.Linq;
	using ArchiMetrics.CodeReview.Rules.Semantic;
	using Microsoft.CodeAnalysis.CSharp.Syntax;
	using NUnit.Framework;


	public sealed class UnreadVariableRuleTests
	{
		private UnreadVariableRuleTests()
		{
		}

		public class GivenAnUnreadVariableRule : SolutionTestsBase
		{
			private UnreadVariableRule _rule;

			[SetUp]
			public void Setup()
			{
				_rule = new UnreadVariableRule();
			}

			[TestCase(@"namespace MyNamespace
{
	public class MyClass
	{
		public MyClass()
		{
			var field = new object();
		}
	}
}")]
			[TestCase(@"namespace MyNamespace
{
	public class MyClass
	{
		public void Something()
		{
			var field =

new object();
		}
	}
}")]
			public void WhenFieldIsNeverReadThenReturnsError(string code)
			{
				var solution = CreateSolution(code);
				var classDeclaration = (from p in solution.Projects
										from d in p.Documents
										let model = d.GetSemanticModelAsync().Result
										let root = d.GetSyntaxRootAsync().Result
										from n in root.DescendantNodes().OfType<VariableDeclarationSyntax>()
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
		public void Write()
		{
			object _field = new object();
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
		public object Get()
		{
			object _field = new object();
			return _field;
		}
	}
}")]
			public void WhenFieldIsReadThenDoesNotReturnError(string code)
			{
				var solution = CreateSolution(code);
				var classDeclaration = (from p in solution.Projects
										from d in p.Documents
										let model = d.GetSemanticModelAsync().Result
										let root = d.GetSyntaxRootAsync().Result
										from n in root.DescendantNodes().OfType<VariableDeclarationSyntax>()
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