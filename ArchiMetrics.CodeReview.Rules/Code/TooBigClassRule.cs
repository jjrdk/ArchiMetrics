namespace ArchiMetrics.CodeReview.Rules.Code
{
	using ArchiMetrics.Common.CodeReview;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;

	internal class TooBigClassRule : CodeEvaluationBase
	{
		private const int Limit = 300;

		public override SyntaxKind EvaluatedKind
		{
			get
			{
				return SyntaxKind.ClassDeclaration;
			}
		}

		public override string Title
		{
			get
			{
				return "Class Too Big";
			}
		}

		public override string Suggestion
		{
			get
			{
				return "Refactor class to make it more manageable.";
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
				return ImpactLevel.Type;
			}
		}

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var declarationSyntax = (TypeDeclarationSyntax)node;
			var snippet = declarationSyntax.ToFullString();
			var linesOfCode = GetLinesOfCode(node);

			if (linesOfCode >= Limit)
			{
				return new EvaluationResult
						   {
							   LinesOfCodeAffected = linesOfCode, 
							   Snippet = snippet
						   };
			}

			return null;
		}
	}
}
