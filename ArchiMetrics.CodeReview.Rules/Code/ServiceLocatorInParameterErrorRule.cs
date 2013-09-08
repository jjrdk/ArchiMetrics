// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceLocatorInParameterErrorRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ServiceLocatorInParameterErrorRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Rules.Code
{
	using ArchiMetrics.Common.CodeReview;
	using Roslyn.Compilers.CSharp;

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
				&& parameterSyntax.Type.Kind == SyntaxKind.IdentifierName
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
