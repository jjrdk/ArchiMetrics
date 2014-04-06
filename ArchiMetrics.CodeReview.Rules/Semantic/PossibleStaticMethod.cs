namespace ArchiMetrics.CodeReview.Rules.Semantic
{
	using System.Threading.Tasks;
	using ArchiMetrics.Analysis;
	using ArchiMetrics.Common.CodeReview;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;

	internal class PossibleStaticMethod : SemanticEvaluationBase
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
				return "Method Can Be Made Static";
			}
		}

		public override string Suggestion
		{
			get
			{
				return "Mark method as static.";
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

		protected override Task<EvaluationResult> EvaluateImpl(
			SyntaxNode node,
			SemanticModel semanticModel,
			Solution solution)
		{
			var method = (MethodDeclarationSyntax)node;
			var analyzer = new SemanticAnalyzer(semanticModel);

			if (analyzer.CanBeMadeStatic(method))
			{
				var snippet = method.ToFullString();
				return Task.FromResult(
					new EvaluationResult
					{
						Snippet = snippet
					});
			}

			return Task.FromResult((EvaluationResult)null);
		}
	}
}