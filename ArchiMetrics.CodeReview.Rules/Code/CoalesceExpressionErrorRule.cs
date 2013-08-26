// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CoalesceExpressionErrorRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the CoalesceExpressionErrorRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Rules.Code
{
	using System.Linq;
	using ArchiMetrics.Common.CodeReview;
	using Roslyn.Compilers.CSharp;

	internal class CoalesceExpressionErrorRule : CodeEvaluationBase
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
				return "Coalesce Expression";
			}
		}
		public override string Suggestion
		{
			get
			{
				return "Use an explicit if statement to assign a value if it is null.";
			}
		}

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var methodDeclaration = (MethodDeclarationSyntax)node;
			var conditionalExpressions = methodDeclaration.DescendantNodes()
														  .Where(n => n.Kind == SyntaxKind.CoalesceExpression)
														  .ToArray();
			if (conditionalExpressions.Any())
			{
				return new EvaluationResult
						   {
							   Quality = CodeQuality.Broken, 
							   ImpactLevel = ImpactLevel.Member, 
							   QualityAttribute = QualityAttribute.Conformance, 
							   Snippet = string.Join("\r\n", conditionalExpressions.Select(n => n.ToFullString())), 
							   ErrorCount = conditionalExpressions.Length
						   };
			}

			return null;
		}
	}
}
