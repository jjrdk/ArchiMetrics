namespace ArchiMetrics.CodeReview.Rules.Code
{
	using System.Linq;
	using ArchiMetrics.Common;
	using ArchiMetrics.Common.CodeReview;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;

	internal class EmptyTestRule : CodeEvaluationBase
	{
		public override SyntaxKind EvaluatedKind
		{
			get { return SyntaxKind.MethodDeclaration; }
		}

		public override string Title
		{
			get
			{
				return "No Assertion in Test";
			}
		}

		public override string Suggestion
		{
			get
			{
				return "Add an assertion to the test.";
			}
		}

		public override CodeQuality Quality
		{
			get
			{
				return CodeQuality.NeedsReview;
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
			var methodParent = (MethodDeclarationSyntax)node;

			if (methodParent != null
				&& methodParent.AttributeLists.Any(
					l => l.Attributes.Any(a => a.Name is SimpleNameSyntax
											   && ((SimpleNameSyntax)a.Name).Identifier.ValueText.IsKnownTestAttribute())))
			{
				if (methodParent.Body == null
					|| !methodParent.Body.ChildNodes().Any())
				{
					return new EvaluationResult
						   {
							   Snippet = (FindClassParent(node) ?? node).ToFullString()
						   };
				}
			}

			return null;
		}
	}
}