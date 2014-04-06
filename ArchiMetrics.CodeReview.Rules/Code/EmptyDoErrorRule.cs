namespace ArchiMetrics.CodeReview.Rules.Code
{
	using System.Linq;
	using ArchiMetrics.Common.CodeReview;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;

	internal class EmptyDoErrorRule : CodeEvaluationBase
	{
		public override SyntaxKind EvaluatedKind
		{
			get { return SyntaxKind.DoStatement; }
		}

		public override string Title
		{
			get
			{
				return "Empty Do Statement";
			}
		}

		public override string Suggestion
		{
			get
			{
				return "Use a wait handle to synchronize asynchronous flows, or let the thread sleep.";
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
				return QualityAttribute.CodeQuality | QualityAttribute.Testability;
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
			var whileStatement = (DoStatementSyntax)node;

			var sleepLoopFound = whileStatement.DescendantNodes()
											   .OfType<BlockSyntax>()
											   .Any(s => !s.ChildNodes().Any());

			if (sleepLoopFound)
			{
				var snippet = FindMethodParent(node).ToFullString();

				return new EvaluationResult
						   {
							   Snippet = snippet
						   };
			}

			return null;
		}
	}
}
