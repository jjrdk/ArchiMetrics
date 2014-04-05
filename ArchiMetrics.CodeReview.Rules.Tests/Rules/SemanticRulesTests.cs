// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SemanticRulesTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the SemanticRulesTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Rules.Tests.Rules
{
	using System;
	using System.Linq;
	using ArchiMetrics.CodeReview.Rules.Semantic;
	using ArchiMetrics.Common.CodeReview;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using NUnit.Framework;


	public class SemanticRulesTests : SolutionTestsBase
	{
		[TestCase(@"public class MyClass { public int GetNumber() { return 1; } }", SyntaxKind.MethodDeclaration, typeof(UnusedMethodRule))]
		[TestCase(@"public class MyClass { public int Number { get { return 1; } } }", SyntaxKind.GetAccessorDeclaration, typeof(UnusedGetPropertyRule))]
		[TestCase(@"public class MyClass { private int _number = 0; public int Number { get { return _number; } set { _number = value; } } }", SyntaxKind.GetAccessorDeclaration, typeof(UnusedSetPropertyRule))]
		public void CanFindUnusedCode(string code, SyntaxKind kind, Type semanticRule)
		{
			var solution = CreateSolution(code);
			var method = solution.Projects.SelectMany(p => p.Documents)
				.SelectMany(d =>
							{
								var semanticModel = d.GetSemanticModelAsync().Result;
								return d.GetSyntaxRootAsync().Result
									.DescendantNodes()
									.OfType<SyntaxNode>()
									.Select(r =>
										new
										{
											node = r,
											model = semanticModel
										});
							})
							.First(x => x.node.IsKind(kind));

			var rule = (ISemanticEvaluation)Activator.CreateInstance(semanticRule);
			var result = rule.Evaluate(method.node, method.model, solution);

			Assert.NotNull(result);
		}
	}
}
