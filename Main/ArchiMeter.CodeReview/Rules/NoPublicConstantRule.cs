// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NoPublicConstantRule.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the NoPublicConstantRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMeter.CodeReview.Rules
{
	using Common;
	using Roslyn.Compilers.CSharp;

	internal class NoPublicConstantRule : EvaluationBase
	{
		public override SyntaxKind EvaluatedKind
		{
			get
			{
				return SyntaxKind.FieldDeclaration;
			}
		}

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var syntax = (FieldDeclarationSyntax)node;
			if (syntax.Modifiers.Any(SyntaxKind.PublicKeyword) && syntax.Modifiers.Any(SyntaxKind.ConstKeyword))
			{
				return new EvaluationResult
						   {
							   Comment = "Public constant declaration", 
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