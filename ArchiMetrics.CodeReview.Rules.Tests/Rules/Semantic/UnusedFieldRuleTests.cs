// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnusedFieldRuleTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the UnusedFieldRuleTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Linq;
using ArchiMetrics.CodeReview.Rules.Semantic;
using NUnit.Framework;
using Roslyn.Compilers.CSharp;

namespace ArchiMetrics.CodeReview.Rules.Tests.Rules.Semantic
{
	public sealed class UnusedFieldRuleTests
	{
		private UnusedFieldRuleTests()
		{
		}

		public class GivenAnUnusedFieldRule : SolutionTestsBase
		{
			private UnusedFieldRule _rule;

			[SetUp]
			public void Setup()
			{
				_rule = new UnusedFieldRule();
			}

			[Test]
			public void WhenFieldIsNeverReadThenReturnsError()
			{
				const string Code = @"namespace MyNamespace
{
	public class MyClass
	{
		private object _field = new object();
	}
}";
				var solution = CreateSolution(Code);
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

			[Test]
			public void WhenFieldIsReadThenDoesNotReturnError()
			{
				const string Code = @"namespace MyNamespace
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
}";
				var solution = CreateSolution(Code);
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