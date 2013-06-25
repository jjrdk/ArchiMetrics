// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VarKeywordRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   VarKeywordRule.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Rules
{
	// internal class VarKeywordRule : CodeEvaluationBase
	// {
	// 	public override SyntaxKind EvaluatedKind
	// 	{
	// 		get { return SyntaxKind.IdentifierName; }
	// 	}

	// 	protected override EvaluationResult EvaluateImpl(SyntaxNode node)
	// 	{
	// 		var identifier = (IdentifierNameSyntax)node;
	// 		if (identifier.IsVar)
	// 		{
	// 			var declaration = GetDeclaration(identifier);
	// 			if (declaration != null)
	// 			{
	// 				return new EvaluationResult
	// 						   {
	// 							   Comment = "var keyword used", 
	// 							   ImpactLevel = ImpactLevel.Line, 
	// 							   Quality = CodeQuality.NeedsReview, 
	// 							   QualityAttribute = QualityAttribute.Conformance, 
	// 							   Snippet = declaration.ToFullString()
	// 						   };
	// 			}
	// 		}

	// 		return null;
	// 	}

	// 	private LocalDeclarationStatementSyntax GetDeclaration(SyntaxNode syntax)
	// 	{
	// 		var declaration = syntax as LocalDeclarationStatementSyntax;
	// 		if (declaration != null)
	// 		{
	// 			return declaration;
	// 		}

	// 		return syntax.Parent == null
	// 				   ? null
	// 				   : GetDeclaration(syntax.Parent);
	// 	}
	// }
}
