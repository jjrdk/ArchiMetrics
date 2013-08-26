// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GotoStatementErrorRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the GotoStatementErrorRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Rules.Code
{
	using ArchiMetrics.Common.CodeReview;
	using Roslyn.Compilers.CSharp;

	internal class GotoStatementErrorRule : CodeEvaluationBase
	{
		public override SyntaxKind EvaluatedKind
		{
			get
			{
				return SyntaxKind.GotoStatement;
			}
		}
		public override string Title
		{
			get
			{
				return "Goto Statement";
			}
		}
		public override string Suggestion
		{
			get
			{
				return "Refactor to use method calls.";
			}
		}

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			return new EvaluationResult
					   {
						   Quality = CodeQuality.Broken, 
						   ImpactLevel = ImpactLevel.Member, 
						   QualityAttribute = QualityAttribute.Conformance, 
						   Snippet = node.ToFullString()
					   };
		}
	}
}
