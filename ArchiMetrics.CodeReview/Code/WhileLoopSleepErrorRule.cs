// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WhileLoopSleepErrorRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the WhileLoopSleepErrorRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Code
{
	using System;
	using System.Linq;
	using ArchiMetrics.Common;
	using Roslyn.Compilers.CSharp;

	internal class WhileLoopSleepErrorRule : CodeEvaluationBase
	{
		public override SyntaxKind EvaluatedKind
		{
			get { return SyntaxKind.WhileStatement; }
		}

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var whileStatement = (WhileStatementSyntax)node;

			var sleepLoopFound = whileStatement.DescendantNodes()
											   .OfType<MemberAccessExpressionSyntax>()
											   .Select(x => new Tuple<SimpleNameSyntax, SimpleNameSyntax>(x.Expression as SimpleNameSyntax, x.Name))
											   .Where(x => x.Item1 != null)
											   .Any(x => x.Item1.Identifier.ValueText == "Thread" && x.Item2.Identifier.ValueText == "Sleep");

			if (sleepLoopFound)
			{
				var snippet = (FindClassParent(node) ?? FindMethodParent(node)).ToFullString();

				return new EvaluationResult
					   {
						   Comment = "Sleep loop found in code.", 
						   Quality = CodeQuality.Incompetent, 
						   QualityAttribute = QualityAttribute.CodeQuality | QualityAttribute.Testability, 
						   Snippet = snippet
					   };
			}

			return null;
		}
	}
}
