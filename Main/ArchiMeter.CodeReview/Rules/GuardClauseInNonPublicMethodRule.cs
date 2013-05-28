// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GuardClauseInNonPublicMethodRule.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the GuardClauseInNonPublicMethodRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMeter.CodeReview.Rules
{
	using Common;
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
							       Snippet = methodParent.ToFullString()
						       };
				}
			}

			return null;
		}
	}
}