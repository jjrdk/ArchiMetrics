// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TooBigMethodRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the TooBigMethodRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Rules.Code
{
	using ArchiMetrics.Common.CodeReview;
	using Roslyn.Compilers.CSharp;

	internal class TooBigMethodRule : CodeEvaluationBase
	{
		private const int Limit = 30;

		public override SyntaxKind EvaluatedKind
		{
			get
			{
				return SyntaxKind.MethodDeclaration;
			}
		}

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var methodDeclaration = (MethodDeclarationSyntax)node;
			var snippet = methodDeclaration.ToFullString();
			var linesOfCode = GetLinesOfCode(snippet);

			if (linesOfCode >= Limit)
			{
				return new EvaluationResult
						   {
							   Comment = "Method too big.",
 							   ImpactLevel = ImpactLevel.Member,
							   Quality = CodeQuality.NeedsRefactoring, 
							   QualityAttribute = QualityAttribute.Testability | QualityAttribute.Maintainability | QualityAttribute.Modifiability, 
							   Snippet = snippet
						   };
			}

			return null;
		}
	}
}
