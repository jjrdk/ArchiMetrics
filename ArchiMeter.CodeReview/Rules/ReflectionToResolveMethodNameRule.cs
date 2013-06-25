// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReflectionToResolveMethodNameRule.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ReflectionToResolveMethodNameRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMeter.CodeReview.Rules
{
	using Common;
	using Roslyn.Compilers.CSharp;

	internal class ReflectionToResolveMethodNameRule : CodeEvaluationBase
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
			if (memberAccess.Expression.Kind == SyntaxKind.InvocationExpression
				&& memberAccess.Expression.GetText().ToString().Trim() == "MethodBase.GetCurrentMethod()")
			{
				return new EvaluationResult
					       {
						       Comment = "Using reflection to find current member name.", 
						       Quality = CodeQuality.Incompetent, 
							   QualityAttribute = QualityAttribute.CodeQuality, 
						       Snippet = node.ToFullString()
					       };
			}

			return null;
		}
	}
}