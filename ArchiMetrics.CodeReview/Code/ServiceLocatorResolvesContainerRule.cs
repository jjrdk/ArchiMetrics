// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceLocatorResolvesContainerRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ServiceLocatorResolvesContainerRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Code
{
	using System.Linq;
	using ArchiMetrics.Common;
	using Roslyn.Compilers.CSharp;

	internal class ServiceLocatorResolvesContainerRule : CodeEvaluationBase
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
			if (memberAccess.Expression.Kind == SyntaxKind.MemberAccessExpression
				&& ((MemberAccessExpressionSyntax)memberAccess.Expression).Expression.Kind == SyntaxKind.IdentifierName
				&& ((IdentifierNameSyntax)((MemberAccessExpressionSyntax)memberAccess.Expression).Expression).Identifier.ValueText == "ServiceLocator"
				&& memberAccess.Name is GenericNameSyntax
				&& memberAccess.Name.Identifier.ValueText == "Resolve"
				&& ((GenericNameSyntax)memberAccess.Name).TypeArgumentList.Arguments.Any(a => a is SimpleNameSyntax && ((SimpleNameSyntax)a).Identifier.ValueText.Contains("UnityContainer")))
			{
				var methodParent = FindMethodParent(node);
				var snippet = methodParent == null
								  ? FindClassParent(node).ToFullString()
								  : methodParent.ToFullString();

				return new EvaluationResult
						   {
							   Comment = "UnityContainer resolved from ServiceLocator.", 
							   Quality = CodeQuality.Broken, 
							   QualityAttribute = QualityAttribute.Testability | QualityAttribute.Maintainability | QualityAttribute.Modifiability | QualityAttribute.Security, 
							   Snippet = snippet
						   };
			}

			return null;
		}
	}
}
