// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CoalesceExpressionErrorRule.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the CoalesceExpressionErrorRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMeter.CodeReview.Rules
{
	using System.Linq;
	using Common;
	using Roslyn.Compilers.CSharp;

	internal class CoalesceExpressionErrorRule : EvaluationBase
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
			var conditionalExpressions = methodDeclaration.DescendantNodes()
			                                              .Where(n => n.Kind == SyntaxKind.CoalesceExpression)
			                                              .ToArray();
			if (conditionalExpressions.Any())
			{
				return new EvaluationResult
					       {
						       Comment = "Coalesce expression found", 
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