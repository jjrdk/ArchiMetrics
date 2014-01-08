// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReflectionToResolveMethodNameRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ReflectionToResolveMethodNameRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Rules.Code
{
	using ArchiMetrics.Common.CodeReview;
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

		public override string Title
		{
			get
			{
				return "Using Reflection to Resolve Member Name";
			}
		}

		public override string Suggestion
		{
			get
			{
				return "Consider using a string for the method name for performance and to make it readable after obfuscation.";
			}
		}

		public override CodeQuality Quality
		{
			get
			{
				return CodeQuality.NeedsCleanup;
			}
		}

		public override QualityAttribute QualityAttribute
		{
			get
			{
				return QualityAttribute.CodeQuality;
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
			var memberAccess = (MemberAccessExpressionSyntax)node;
			if (memberAccess.Expression.Kind == SyntaxKind.InvocationExpression
				&& memberAccess.Expression.GetText().ToString().Trim() == "MethodBase.GetCurrentMethod()")
			{
				return new EvaluationResult
						   {
							   Snippet = node.ToFullString()
						   };
			}

			return null;
		}
	}
}
