// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TooBigClassRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the TooBigClassRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Rules.Code
{
	using ArchiMetrics.Common.CodeReview;
	using Roslyn.Compilers.CSharp;

	internal class TooBigClassRule : CodeEvaluationBase
	{
		private const int Limit = 400;

		public override SyntaxKind EvaluatedKind
		{
			get
			{
				return SyntaxKind.ClassDeclaration;
			}
		}

		public override string Title
		{
			get
			{
				return "Class Too Big";
			}
		}

		public override string Suggestion
		{
			get
			{
				return "Refactor class to make it more manageable.";
			}
		}

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var declarationSyntax = (TypeDeclarationSyntax)node;
			var snippet = declarationSyntax.ToFullString();
			var linesOfCode = GetLinesOfCode(snippet);

			if (linesOfCode >= Limit)
			{
				return new EvaluationResult
						   {
							   Comment = "Class too big.",
							   ImpactLevel = ImpactLevel.Type,
							   Quality = CodeQuality.NeedsRefactoring,
							   QualityAttribute = QualityAttribute.Testability | QualityAttribute.Maintainability | QualityAttribute.Modifiability,
							   Snippet = snippet
						   };
			}

			return null;
		}
	}
}
