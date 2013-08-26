// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NoPublicConstantRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the NoPublicConstantRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Rules.Code
{
	using ArchiMetrics.Common.CodeReview;
	using Roslyn.Compilers.CSharp;

	internal class NoPublicConstantRule : CodeEvaluationBase
	{
		public override SyntaxKind EvaluatedKind
		{
			get
			{
				return SyntaxKind.FieldDeclaration;
			}
		}
		public override string Title
		{
			get
			{
				return "No Public Constants";
			}
		}
		public override string Suggestion
		{
			get
			{
				return "Expose public constants as public static readonly instead in order to avoid that they get compiled into a calling assembly.";
			}
		}

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var syntax = (FieldDeclarationSyntax)node;
			if (syntax.Modifiers.Any(SyntaxKind.PublicKeyword) && syntax.Modifiers.Any(SyntaxKind.ConstKeyword))
			{
				return new EvaluationResult
						   {
							   Quality = CodeQuality.Broken, 
							   QualityAttribute = QualityAttribute.Modifiability, 
							   Snippet = node.ToFullString(), 
							   ImpactLevel = ImpactLevel.Project
						   };
			}

			return null;
		}
	}
}
