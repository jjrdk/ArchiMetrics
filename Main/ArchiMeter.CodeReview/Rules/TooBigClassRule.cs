// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TooBigClassRule.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the TooBigClassRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMeter.CodeReview.Rules
{
	using Common;
	using Roslyn.Compilers.CSharp;

	internal class TooBigClassRule : EvaluationBase
	{
		private const int Limit = 400;
		
		public override SyntaxKind EvaluatedKind
		{
			get
			{
				return SyntaxKind.ClassDeclaration;
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
							   Comment = "Class too big", 
						       Quality = CodeQuality.NeedsRefactoring, 
							   QualityAttribute = QualityAttribute.Testability | QualityAttribute.Maintainability | QualityAttribute.Modifiability, 
						       Snippet = snippet
					       };
			}

			return null;
		}
	}
}