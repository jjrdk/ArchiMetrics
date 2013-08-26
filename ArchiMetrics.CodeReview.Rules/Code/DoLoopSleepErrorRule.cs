// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DoLoopSleepErrorRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the DoLoopSleepErrorRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Rules.Code
{
	using System;
	using System.Linq;
	using ArchiMetrics.Common.CodeReview;
	using Roslyn.Compilers.CSharp;

	internal class DoLoopSleepErrorRule : CodeEvaluationBase
	{
		public override SyntaxKind EvaluatedKind
		{
			get { return SyntaxKind.DoStatement; }
		}
		public override string Title
		{
			get
			{
				return "Do Statement Sleep Loop";
			}
		}
		public override string Suggestion
		{
			get
			{
				return "Use a wait handle to synchronize timing issues.";
			}
		}

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var statement = (DoStatementSyntax)node;

			var sleepLoopFound = statement.DescendantNodes()
											.OfType<MemberAccessExpressionSyntax>()
											.Select(x => new Tuple<SimpleNameSyntax, SimpleNameSyntax>(x.Expression as SimpleNameSyntax, x.Name))
											.Where(x => x.Item1 != null)
											.Any(x => x.Item1.Identifier.ValueText == "Thread" && x.Item2.Identifier.ValueText == "Sleep");

			if (sleepLoopFound)
			{
				var snippet = FindMethodParent(node).ToFullString();

				return new EvaluationResult
					   {
						   Quality = CodeQuality.Incompetent,
						   QualityAttribute = QualityAttribute.CodeQuality | QualityAttribute.Testability,
						   ImpactLevel = ImpactLevel.Type,
						   Snippet = snippet
					   };
			}

			return null;
		}
	}
}
