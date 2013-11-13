// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceLocatorInvocationRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ServiceLocatorInvocationRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Rules.Code
{
	using ArchiMetrics.Common.CodeReview;
	using Roslyn.Compilers.CSharp;

	internal class ServiceLocatorInvocationRule : CodeEvaluationBase
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
				return "ServiceLocator Invocation";
			}
		}

		public override string Suggestion
		{
			get
			{
				return "Consider injecting needed dependencies explicitly.";
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
				return QualityAttribute.Testability | QualityAttribute.Maintainability | QualityAttribute.Modifiability;
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
			var memberAccess = (MemberAccessExpressionSyntax)node;
			if (memberAccess.Expression.Kind == SyntaxKind.MemberAccessExpression
				&& ((MemberAccessExpressionSyntax)memberAccess.Expression).Expression.Kind == SyntaxKind.IdentifierName
				&& ((IdentifierNameSyntax)((MemberAccessExpressionSyntax)memberAccess.Expression).Expression).Identifier.ValueText == "ServiceLocator"
				&& memberAccess.Name.Identifier.ValueText == "Resolve")
			{
				var methodParent = FindMethodParent(node);
				var snippet = methodParent == null
					? FindClassParent(node).ToFullString()
					: methodParent.ToFullString();

				return new EvaluationResult
				{
					Snippet = snippet
				};
			}

			return null;
		}
	}
}
