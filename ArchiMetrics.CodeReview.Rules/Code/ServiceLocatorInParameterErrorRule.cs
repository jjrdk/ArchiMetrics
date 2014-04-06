namespace ArchiMetrics.CodeReview.Rules.Code
{
	using ArchiMetrics.Common.CodeReview;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;

	internal class ServiceLocatorInParameterErrorRule : CodeEvaluationBase
	{
		public override SyntaxKind EvaluatedKind
		{
			get
			{
				return SyntaxKind.Parameter;
			}
		}

		public override string Title
		{
			get
			{
				return "ServiceLocator Passed as Parameter";
			}
		}

		public override string Suggestion
		{
			get
			{
				return "Remove ServiceLocator parameter and inject only needed dependencies.";
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
				return QualityAttribute.Maintainability | QualityAttribute.Modifiability | QualityAttribute.Reusability | QualityAttribute.Testability;
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
			var parameterSyntax = (ParameterSyntax)node;
			if (parameterSyntax.Type != null
				&& parameterSyntax.Type.IsKind(SyntaxKind.IdentifierName)
				&& ((IdentifierNameSyntax)parameterSyntax.Type).Identifier.ValueText.Contains("ServiceLocator"))
			{
				var parentMethod = FindMethodParent(parameterSyntax);
				var snippet = parentMethod == null
								  ? parameterSyntax.Parent.Parent.ToFullString()
								  : parentMethod is ConstructorDeclarationSyntax
										? FindClassParent(parameterSyntax).ToFullString()
										: parentMethod.ToFullString();

				return new EvaluationResult
				{
					Snippet = snippet
				};
			}

			return null;
		}
	}
}
