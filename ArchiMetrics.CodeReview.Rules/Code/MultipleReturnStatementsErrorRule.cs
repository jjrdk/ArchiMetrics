// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MultipleReturnStatementsErrorRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the MultipleReturnStatementsErrorRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Rules.Code
{
	using System.Linq;
	using ArchiMetrics.Common.CodeReview;
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
		public override string Title
		{
			get
			{
				return "Multiple Return Statements";
			}
		}
		public override string Suggestion
		{
			get
			{
				return "If your company's coding standards requires only a single exit point, then refactor method to have only single return statement.";
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
