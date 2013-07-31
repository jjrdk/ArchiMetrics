// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DoNotDestroyStackTraceRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the DoNotDestroyStackTraceRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Code
{
	using System.Linq;

	using ArchiMetrics.Common;

	using Roslyn.Compilers.CSharp;

	internal class DoNotDestroyStackTraceRule : CodeEvaluationBase
	{
		public override SyntaxKind EvaluatedKind
		{
			get
			{
				return SyntaxKind.CatchClause;
			}
		}

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var catchClause = (CatchClauseSyntax)node;
			var catchesException = catchClause
				.DescendantNodesAndSelf()
				.OfType<CatchDeclarationSyntax>()
				.SelectMany(x => x.DescendantNodes())
				.OfType<IdentifierNameSyntax>()
				.Any(x => x.Identifier.ValueText == "Exception");
			var throwsSomething = catchClause
				.DescendantNodes()
				.OfType<ThrowStatementSyntax>()
				.Any(x => x.Expression != null);
			if (catchesException && throwsSomething)
			{
				var result = new EvaluationResult
							 {
								 Comment = "Stack trace destroyed",
								 ImpactLevel = ImpactLevel.Member,
								 Quality = CodeQuality.Incompetent,
								 QualityAttribute = QualityAttribute.CodeQuality,
								 Snippet = catchClause.ToFullString()
							 };
				return result;
			}

			return null;
		}
	}
}