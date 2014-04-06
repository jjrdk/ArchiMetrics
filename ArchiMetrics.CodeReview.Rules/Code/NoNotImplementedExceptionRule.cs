namespace ArchiMetrics.CodeReview.Rules.Code
{
	using ArchiMetrics.Common.CodeReview;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;

	internal class NoNotImplementedExceptionRule : CodeEvaluationBase
	{
		public override SyntaxKind EvaluatedKind
		{
			get { return SyntaxKind.ThrowStatement; }
		}

		public override string Title
		{
			get { return "NotImplementedException Thrown"; }
		}

		public override string Suggestion
		{
			get { return "Add method implementation."; }
		}

		public override CodeQuality Quality
		{
			get { return CodeQuality.Broken; }
		}

		public override QualityAttribute QualityAttribute
		{
			get { return QualityAttribute.CodeQuality; }
		}

		public override ImpactLevel ImpactLevel
		{
			get { return ImpactLevel.Member; }
		}

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var statement = (ThrowStatementSyntax)node;
			var exceptionCreation = statement.Expression as ObjectCreationExpressionSyntax;
			if (exceptionCreation != null)
			{
				var exceptionType = exceptionCreation.Type as IdentifierNameSyntax;
				if (exceptionType != null && exceptionType.Identifier.ValueText.EndsWith("NotImplementedException"))
				{
					return new EvaluationResult
					{
						Snippet = node.ToFullString()
					};
				}
			}

			return null;
		}
	}
}