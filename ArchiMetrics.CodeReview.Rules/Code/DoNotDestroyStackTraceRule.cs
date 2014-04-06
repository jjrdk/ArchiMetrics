namespace ArchiMetrics.CodeReview.Rules.Code
{
	using System.Linq;
	using ArchiMetrics.Common.CodeReview;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;

	internal class DoNotDestroyStackTraceRule : CodeEvaluationBase
	{
		public override SyntaxKind EvaluatedKind
		{
			get
			{
				return SyntaxKind.CatchClause;
			}
		}

		public override string Title
		{
			get
			{
				return "Stack Trace Destroyed";
			}
		}

		public override string Suggestion
		{
			get
			{
				return "Use only 'throw' to rethrow the original stack trace.";
			}
		}

		public override CodeQuality Quality
		{
			get
			{
				return CodeQuality.NeedsCleanup;
			}
		}

		public override QualityAttribute QualityAttribute
		{
			get
			{
				return QualityAttribute.CodeQuality;
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
			var catchClause = (CatchClauseSyntax)node;
			var catchesException = catchClause
				.DescendantNodesAndSelf()
				.OfType<CatchDeclarationSyntax>()
				.SelectMany(x => x.DescendantNodes())
				.OfType<IdentifierNameSyntax>()
				.Any(x => x.Identifier.ValueText == "Exception");
			var throwsSomething = catchClause
				.DescendantNodes()
				.OfType<ThrowStatementSyntax>()
				.Any(x => x.Expression != null);
			if (catchesException && throwsSomething)
			{
				var result = new EvaluationResult
							 {
								 Snippet = catchClause.ToFullString()
							 };
				return result;
			}

			return null;
		}
	}
}