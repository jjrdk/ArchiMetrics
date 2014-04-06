namespace ArchiMetrics.CodeReview.Rules.Code
{
	using System;
	using System.Linq;
	using ArchiMetrics.Common.CodeReview;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;

	internal class DoLoopSleepErrorRule : CodeEvaluationBase
	{
		public override SyntaxKind EvaluatedKind
		{
			get { return SyntaxKind.DoStatement; }
		}

		public override string Title
		{
			get
			{
				return "Do Statement Sleep Loop";
			}
		}
		
		public override string Suggestion
		{
			get
			{
				return "Use a wait handle to synchronize timing issues.";
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
			var statement = (DoStatementSyntax)node;

			var sleepLoopFound = statement.DescendantNodes()
											.OfType<MemberAccessExpressionSyntax>()
											.Select(x => new Tuple<SimpleNameSyntax, SimpleNameSyntax>(x.Expression as SimpleNameSyntax, x.Name))
											.Where(x => x.Item1 != null)
											.Any(x => x.Item1.Identifier.ValueText == "Thread" && x.Item2.Identifier.ValueText == "Sleep");

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
