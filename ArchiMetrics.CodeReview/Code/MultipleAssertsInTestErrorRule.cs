// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MultipleAssertsInTestErrorRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the MultipleAssertsInTestErrorRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMetrics.CodeReview.Rules
{
	using System.Linq;
	using Common;
	using Roslyn.Compilers.CSharp;

	internal class MultipleAssertsInTestErrorRule : CodeEvaluationBase
	{
		public override SyntaxKind EvaluatedKind
		{
			get
			{
				return SyntaxKind.MethodDeclaration;
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
				return total >= 0
						   ? new EvaluationResult
								 {
									 Comment = "Multiple asserts found in test.", // assertsFound + " asserts found in test.",
									 Quality = CodeQuality.Broken, 
									 QualityAttribute = QualityAttribute.Testability | QualityAttribute.CodeQuality, 
									 Snippet = node.ToFullString(), 
									 ErrorCount = total
								 }
						   : null;
			}

			return null;
		}
	}
}
