namespace ArchiMetrics.CodeReview.Rules.Code
{
	using ArchiMetrics.Analysis.Metrics;
	using ArchiMetrics.Common.CodeReview;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;

	internal class TooHighCyclomaticComplexityRule : CodeEvaluationBase
	{
		private const int Limit = 10;
		private readonly CyclomaticComplexityCounter _counter = new CyclomaticComplexityCounter();

		public override SyntaxKind EvaluatedKind
		{
			get { return SyntaxKind.MethodDeclaration; }
		}

		public override string Title
		{
			get
			{
				return "Method Too Complex.";
			}
		}

		public override string Suggestion
		{
			get
			{
				return "Refactor to reduce number of code paths through method.";
			}
		}

		public override CodeQuality Quality
		{
			get
			{
				return CodeQuality.NeedsRefactoring;
			}
		}

		public override QualityAttribute QualityAttribute
		{
			get
			{
				return QualityAttribute.Testability | QualityAttribute.Maintainability | QualityAttribute.Modifiability;
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
			var complexity = _counter.Calculate(methodDeclaration, null);
			if (complexity >= Limit)
			{
				return new EvaluationResult
						   {
							   Snippet = node.ToFullString()
						   };
			}

			return null;
		}
	}
}
