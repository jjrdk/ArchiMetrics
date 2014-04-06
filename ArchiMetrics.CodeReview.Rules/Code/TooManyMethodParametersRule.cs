namespace ArchiMetrics.CodeReview.Rules.Code
{
	using ArchiMetrics.Common.CodeReview;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;

	internal class TooManyMethodParametersRule : CodeEvaluationBase
	{
		private const int Limit = 5;

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
				return "More than " + Limit + " parameters on method";
			}
		}

		public override string Suggestion
		{
			get
			{
				return "Refactor method to reduce number of dependencies passed.";
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
			var parameterCount = methodDeclaration.ParameterList.Parameters.Count;

			if (parameterCount >= Limit)
			{
				return new EvaluationResult
						   {
							   ErrorCount = parameterCount, 
							   Snippet = methodDeclaration.ToFullString()
						   };
			}

			return null;
		}
	}
}
