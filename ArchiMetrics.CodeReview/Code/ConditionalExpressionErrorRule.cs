// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConditionalExpressionErrorRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ConditionalExpressionErrorRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMetrics.CodeReview.Rules
{
	using System.Linq;
	using Common;
	using Roslyn.Compilers.CSharp;

	internal class ConditionalExpressionErrorRule : CodeEvaluationBase
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
			                                              .Where(n => n.Kind == SyntaxKind.ConditionalExpression)
			                                              .ToArray();
			if (conditionalExpressions.Any())
			{
				return new EvaluationResult
					       {
						       Comment = "Conditional expression found", 
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
