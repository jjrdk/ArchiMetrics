// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MultipleReturnStatementsErrorRule.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the MultipleReturnStatementsErrorRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMetrics.CodeReview.Rules
{
	using System.Linq;
	using Common;
	using Roslyn.Compilers.CSharp;

	internal class MultipleReturnStatementsErrorRule : CodeEvaluationBase
	{
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
			var returnStatements = methodDeclaration.DescendantNodes().Where(n => n.Kind == SyntaxKind.ReturnStatement).ToArray();
			if (returnStatements.Length > 1)
			{
				return new EvaluationResult
						   {
							   Comment = "Multiple return statements", 
							   Quality = CodeQuality.Broken, 
							   ImpactLevel = ImpactLevel.Member, 
							   QualityAttribute = QualityAttribute.Conformance, 
							   Snippet = node.ToFullString(), 
							   ErrorCount = returnStatements.Length
						   };
			}

			return null;
		}
	}
}
