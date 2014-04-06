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
