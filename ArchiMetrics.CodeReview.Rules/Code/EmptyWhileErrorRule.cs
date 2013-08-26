// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EmptyWhileErrorRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the EmptyWhileErrorRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Rules.Code
{
	using System.Linq;
	using ArchiMetrics.Common.CodeReview;
	using Roslyn.Compilers.CSharp;

	internal class EmptyWhileErrorRule : CodeEvaluationBase
	{
		public override SyntaxKind EvaluatedKind
		{
			get { return SyntaxKind.WhileStatement; }
		}
		public override string Title
		{
			get
			{
				return "Empty While Statement";
			}
		}
		public override string Suggestion
		{
			get
			{
				return "Use a wait handle to synchronize asynchronous flows, or let the thread sleep.";
			}
		}

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var whileStatement = (WhileStatementSyntax)node;

			var sleepLoopFound = whileStatement.DescendantNodes()
											   .OfType<BlockSyntax>()
											   .Any(s => !s.ChildNodes().Any());

			if (sleepLoopFound)
			{
				var snippet = FindMethodParent(node).ToFullString();

				return new EvaluationResult
					   {
						   Quality = CodeQuality.Incompetent, 
						   QualityAttribute = QualityAttribute.CodeQuality | QualityAttribute.Testability, 
						   ImpactLevel = ImpactLevel.Member,
						   Snippet = snippet
					   };
			}

			return null;
		}
	}
}
