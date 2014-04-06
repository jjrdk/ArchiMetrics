namespace ArchiMetrics.CodeReview.Rules.Code
{
	using ArchiMetrics.Common.CodeReview;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;

	internal class NoProtectedFieldsInPublicClassesRule : CodeEvaluationBase
	{
		public override SyntaxKind EvaluatedKind
		{
			get
			{
				return SyntaxKind.FieldDeclaration;
			}
		}

		public override string Title
		{
			get
			{
				return "No Protected Fields";
			}
		}

		public override string Suggestion
		{
			get
			{
				return "Encapsulate all public fields in properties, or internalize them.";
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
				return QualityAttribute.Modifiability;
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
			var classParent = FindClassParent(node);
			if (classParent != null && classParent.Modifiers.Any(SyntaxKind.PublicKeyword))
			{
				var syntax = (FieldDeclarationSyntax)node;
				if (syntax.Modifiers.Any(SyntaxKind.ProtectedKeyword))
				{
					return new EvaluationResult
							   {
								   Snippet = classParent.ToFullString()
							   };
				}
			}

			return null;
		}
	}
}
