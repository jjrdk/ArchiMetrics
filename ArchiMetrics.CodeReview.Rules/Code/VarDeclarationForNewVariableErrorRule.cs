// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VarDeclarationForNewVariableErrorRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the VarDeclarationForNewVariableErrorRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Rules.Code
{
	using System.Linq;
	using ArchiMetrics.Common.CodeReview;
	using Roslyn.Compilers.CSharp;

	internal class VarDeclarationForNewVariableErrorRule : CodeEvaluationBase
	{
		public override SyntaxKind EvaluatedKind
		{
			get
			{
				return SyntaxKind.VariableDeclaration;
			}
		}

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var declaration = (VariableDeclarationSyntax)node;
			if (declaration.Type.IsVar && !declaration.Variables.All(x => x.Initializer.Value is ObjectCreationExpressionSyntax))
			{
				return new EvaluationResult
						   {
							   Comment = "Var used for new variable.", 
							   ErrorCount = declaration.Variables.Count, 
							   ImpactLevel = ImpactLevel.Line, 
							   Quality = CodeQuality.Broken, 
							   QualityAttribute = QualityAttribute.Conformance, 
							   Snippet = declaration.ToFullString()
						   };
			}

			return null;
		}
	}
}
