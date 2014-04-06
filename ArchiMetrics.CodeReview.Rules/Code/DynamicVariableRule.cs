namespace ArchiMetrics.CodeReview.Rules.Code
{
	using ArchiMetrics.Common.CodeReview;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;

	internal class DynamicVariableRule : CodeEvaluationBase
	{
		public override SyntaxKind EvaluatedKind
		{
			get
			{
				return SyntaxKind.VariableDeclaration;
			}
		}

		public override string Title
		{
			get
			{
				return "Dynamic Variable";
			}
		}

		public override string Suggestion
		{
			get
			{
				return "Consider using a typed variable.";
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
			var variableDeclaration = (VariableDeclarationSyntax)node;
			if (variableDeclaration.Type.GetText().ToString().Trim() == "dynamic")
			{
				var methodParent = FindMethodParent(node);
				var snippet = methodParent.ToFullString();

				return new EvaluationResult
						   {
							   Snippet = snippet
						   };
			}

			return null;
		}
	}
}
