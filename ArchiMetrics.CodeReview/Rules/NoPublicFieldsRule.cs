// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NoPublicFieldsRule.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the NoPublicFieldsRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMetrics.CodeReview.Rules
{
	using Common;
	using Roslyn.Compilers.CSharp;

	internal class NoPublicFieldsRule : CodeEvaluationBase
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
			if (syntax.Modifiers.Any(SyntaxKind.PublicKeyword))
			{
				return new EvaluationResult
						   {
							   Comment = "Public field declaration", 
							   Quality = CodeQuality.Broken, 
							   QualityAttribute = QualityAttribute.Modifiability, 
							   Snippet = node.ToFullString(), 
							   ImpactLevel = ImpactLevel.Type
						   };
			}

			return null;
		}
	}
}
