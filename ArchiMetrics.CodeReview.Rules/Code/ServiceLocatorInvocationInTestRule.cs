// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceLocatorInvocationInTestRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ServiceLocatorInvocationInTestRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Rules.Code
{
	using System.Linq;
	using ArchiMetrics.Common.CodeReview;
	using Roslyn.Compilers.CSharp;

	internal class ServiceLocatorInvocationInTestRule : CodeEvaluationBase
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
				return "ServiceLocator Invocation in Test";
			}
		}
		public override string Suggestion
		{
			get
			{
				return "Replace ServiceLocator with explicit setup using either a concrete instance, mock or fake.";
			}
		}

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var memberAccess = (MemberAccessExpressionSyntax)node;
			if (memberAccess.Expression.Kind == SyntaxKind.MemberAccessExpression
				&& ((MemberAccessExpressionSyntax)memberAccess.Expression).Expression.Kind == SyntaxKind.IdentifierName
				&& ((IdentifierNameSyntax)((MemberAccessExpressionSyntax)memberAccess.Expression).Expression).Identifier.ValueText == "ServiceLocator"
				&& memberAccess.Name.Identifier.ValueText == "Resolve")
			{
				var methodParent = FindMethodParent(node) as MethodDeclarationSyntax;
				if (methodParent != null
					&& methodParent.AttributeLists != null
					&& methodParent.AttributeLists.Any(l => l.Attributes.Any(a => a.Name is SimpleNameSyntax && ((SimpleNameSyntax)a.Name).Identifier.ValueText.IsKnownTestAttribute())))
				{
					var snippet = methodParent.ToFullString();

					return new EvaluationResult
							   {
								   Comment = "ServiceLocator invocation found in test method.", 
								   Quality = CodeQuality.Incompetent, 
								   QualityAttribute = QualityAttribute.CodeQuality | QualityAttribute.Maintainability | QualityAttribute.Modifiability, 
								   Snippet = snippet, 
								   ImpactLevel = ImpactLevel.Member
							   };
				}
			}

			return null;
		}
	}
}
