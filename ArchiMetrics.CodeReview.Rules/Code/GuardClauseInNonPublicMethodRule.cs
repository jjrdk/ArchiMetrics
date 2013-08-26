// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GuardClauseInNonPublicMethodRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the GuardClauseInNonPublicMethodRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Rules.Code
{
	using ArchiMetrics.Common.CodeReview;
	using Roslyn.Compilers.CSharp;

	internal class GuardClauseInNonPublicMethodRule : CodeEvaluationBase
	{
		public override SyntaxKind EvaluatedKind
		{
			get
			{
				return SyntaxKind.MemberAccessExpression;
			}
		}

		public override string Title
		{
			get
			{
				return "Guard Clause in Non-Public Method.";
			}
		}

		public override string Suggestion
		{
			get
			{
				return "Remove Guard clause and verify internal state by other means.";
			}
		}

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var memberAccess = (MemberAccessExpressionSyntax)node;
			if (memberAccess.Expression.Kind == SyntaxKind.IdentifierName
				&& ((IdentifierNameSyntax)memberAccess.Expression).Identifier.ValueText == "Guard")
			{
				var methodParent = FindMethodParent(node) as MethodDeclarationSyntax;
				if (methodParent != null && !methodParent.Modifiers.Any(SyntaxKind.PublicKeyword))
				{
					return new EvaluationResult
							   {
								   Comment = "Guard clause in non-public method.",
								   Quality = CodeQuality.Incompetent,
								   QualityAttribute = QualityAttribute.CodeQuality,
								   ImpactLevel = ImpactLevel.Member,
								   Snippet = methodParent.ToFullString()
							   };
				}
			}

			return null;
		}
	}
}
