namespace ArchiMetrics.CodeReview.Rules.Code
{
	using ArchiMetrics.Common.CodeReview;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;

	internal class LocalTimeCreationRule : CodeEvaluationBase
	{
		public override SyntaxKind EvaluatedKind
		{
			get
			{
				return SyntaxKind.SimpleMemberAccessExpression;
			}
		}

		public override string Title
		{
			get
			{
				return "Local Time Creation";
			}
		}

		public override string Suggestion
		{
			get
			{
				return "Replace with call to DateTime.UtcNow";
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
				return QualityAttribute.Conformance;
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
			var memberAccess = (MemberAccessExpressionSyntax)node;
			if (memberAccess.Expression.IsKind(SyntaxKind.IdentifierName)
				&& ((IdentifierNameSyntax)memberAccess.Expression).Identifier.ValueText == "DateTime"
				&& memberAccess.Name.Identifier.ValueText == "Now")
			{
				var methodParent = FindMethodParent(node);
				var snippet = methodParent == null
								  ? node.ToFullString()
								  : methodParent.ToFullString();

				return new EvaluationResult
						   {
							   Snippet = snippet
						   };
			}

			return null;
		}
	}
}
