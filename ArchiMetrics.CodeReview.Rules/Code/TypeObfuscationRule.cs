namespace ArchiMetrics.CodeReview.Rules.Code
{
	using System.Linq;
	using ArchiMetrics.Common.CodeReview;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;

	internal class TypeObfuscationRule : CodeEvaluationBase
	{
		public override SyntaxKind EvaluatedKind
		{
			get
			{
				return SyntaxKind.LocalDeclarationStatement;
			}
		}

		public override string Title
		{
			get
			{
				return "Type Obfuscation";
			}
		}

		public override string Suggestion
		{
			get
			{
				return "Assigning a value to a variable of type object bypasses type checking.";
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
			var declaration = ((LocalDeclarationStatementSyntax)node).Declaration;

			var declarationString = declaration.Type.ToFullString().Trim();
			var objectString = SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.ObjectKeyword)).ToFullString().Trim();
			if (declarationString.Equals(objectString)
				&& declaration.Variables.Any(v => v.Initializer == null || v.Initializer.Value.IsKind(SyntaxKind.NullLiteralExpression)))
			{
				return new EvaluationResult
						   {
							   Snippet = (FindMethodParent(node) ?? FindClassParent(node)).ToFullString()
						   };
			}

			return null;
		}
	}
}
