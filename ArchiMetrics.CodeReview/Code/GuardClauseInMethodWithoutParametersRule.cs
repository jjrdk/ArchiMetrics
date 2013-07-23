// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GuardClauseInMethodWithoutParametersRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the GuardClauseInMethodWithoutParametersRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMetrics.CodeReview.Code
{
	using Common;
	using Roslyn.Compilers.CSharp;

	internal class GuardClauseInMethodWithoutParametersRule : CodeEvaluationBase
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
				if (methodParent != null && (methodParent.ParameterList == null || methodParent.ParameterList.Parameters.Count == 0))
				{
					return new EvaluationResult
							   {
								   Comment = "Guard clause without parameters.", 
								   Quality = CodeQuality.Broken, 
								   QualityAttribute = QualityAttribute.CodeQuality, 
								   Snippet = methodParent.ToFullString()
							   };
				}
			}

			return null;
		}
	}
}
