// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TooBigClassRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
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
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;

	internal class TooBigClassRule : CodeEvaluationBase
	{
		private const int Limit = 300;

		public override string ID
		{
			get
			{
				return "AM0043";
			}
		}

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
				return ImpactLevel.Type;
			}
		}

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var declarationSyntax = (TypeDeclarationSyntax)node;
			var snippet = declarationSyntax.ToFullString();
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
