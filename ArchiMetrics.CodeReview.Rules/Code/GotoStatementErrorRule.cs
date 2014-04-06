namespace ArchiMetrics.CodeReview.Rules.Code
{
	using ArchiMetrics.Common.CodeReview;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;

	internal class GotoStatementErrorRule : CodeEvaluationBase
	{
		public override SyntaxKind EvaluatedKind
		{
			get
			{
				return SyntaxKind.GotoStatement;
			}
		}

		public override string Title
		{
			get
			{
				return "Goto Statement";
			}
		}

		public override string Suggestion
		{
			get
			{
				return "Refactor to use method calls.";
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
			return new EvaluationResult
					   {
						   Snippet = node.ToFullString()
					   };
		}
	}
}
