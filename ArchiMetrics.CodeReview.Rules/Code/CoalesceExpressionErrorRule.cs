namespace ArchiMetrics.CodeReview.Rules.Code
{
	using System.Linq;
	using ArchiMetrics.Common.CodeReview;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;

	internal class CoalesceExpressionErrorRule : CodeEvaluationBase
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
				return "Coalesce Expression";
			}
		}

		public override string Suggestion
		{
			get
			{
				return "Use an explicit if statement to assign a value if it is null.";
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
			var methodDeclaration = (MethodDeclarationSyntax)node;
			var conditionalExpressions = methodDeclaration.DescendantNodes()
														  .Where(n => n.IsKind(SyntaxKind.CoalesceExpression))
														  .ToArray();
			if (conditionalExpressions.Any())
			{
				return new EvaluationResult
						   {
							   Snippet = string.Join("\r\n", conditionalExpressions.Select(n => n.ToFullString())),
							   ErrorCount = conditionalExpressions.Length
						   };
			}

			return null;
		}
	}
}
