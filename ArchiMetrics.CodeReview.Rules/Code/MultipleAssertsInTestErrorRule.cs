// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MultipleAssertsInTestErrorRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the MultipleAssertsInTestErrorRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Rules.Code
{
	using System.Linq;
	using ArchiMetrics.Common.CodeReview;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;

	internal class MultipleAssertsInTestErrorRule : CodeEvaluationBase
	{
		public override SyntaxKind EvaluatedKind
		{
			get
			{
				return SyntaxKind.MethodDeclaration;
			}
		}

		public override string Title
		{
			get
			{
				return "Multiple Asserts in Test";
			}
		}

		public override string Suggestion
		{
			get
			{
				return "Refactor tests to only have a single assert.";
			}
		}

		public override CodeQuality Quality
		{
			get
			{
				return CodeQuality.Broken;
			}
		}

		public override QualityAttribute QualityAttribute
		{
			get
			{
				return QualityAttribute.Testability | QualityAttribute.CodeQuality;
			}
		}

		public override ImpactLevel ImpactLevel
		{
			get
			{
				return ImpactLevel.Member;
			}
		}

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var methodDeclaration = (MethodDeclarationSyntax)node;

			if (methodDeclaration.AttributeLists.Any(l => l.Attributes.Any(a => a.Name is SimpleNameSyntax && ((SimpleNameSyntax)a.Name).Identifier.ValueText == "TestMethod")))
			{
				var assertsFound = methodDeclaration.DescendantNodes()
													.OfType<MemberAccessExpressionSyntax>()
													.Select(x => x.Expression as SimpleNameSyntax)
													.Where(x => x != null)
													.Count(x => x.Identifier.ValueText == "Assert" || x.Identifier.ValueText == "ExceptionAssert");
				var mockVerifyFound = methodDeclaration.DescendantNodes()
													   .OfType<MemberAccessExpressionSyntax>()
													   .Count(x => x.Name.Identifier.ValueText == "Verify" || x.Name.Identifier.ValueText == "VerifySet" || x.Name.Identifier.ValueText == "VerifyGet");
				var expectedExceptions =
					methodDeclaration.AttributeLists.Count(
						l =>
						l.Attributes.Any(a => a.Name is SimpleNameSyntax && ((SimpleNameSyntax)a.Name).Identifier.ValueText == "ExpectedException"));

				var total = assertsFound + mockVerifyFound + expectedExceptions;
				return total != 1
						   ? new EvaluationResult
								 {
									 Snippet = node.ToFullString(), 
									 ErrorCount = total
								 }
						   : null;
			}

			return null;
		}
	}
}
