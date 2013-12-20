// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TooBigMethodRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
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

		public override string Title
		{
			get
			{
				return "Method Too Big";
			}
		}

		public override string Suggestion
		{
			get
			{
				return "Refactor method to make it more manageable.";
			}
		}

		public override CodeQuality Quality
		{
			get
			{
				return CodeQuality.NeedsRefactoring;
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
				return ImpactLevel.Member;
			}
		}

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var methodDeclaration = (MethodDeclarationSyntax)node;
			var snippet = methodDeclaration.ToFullString();
			var linesOfCode = GetLinesOfCode(node);

			if (linesOfCode >= Limit)
			{
				return new EvaluationResult
						   {
							   LinesOfCodeAffected = linesOfCode,
							   Snippet = snippet
						   };
			}

			return null;
		}
	}
}
