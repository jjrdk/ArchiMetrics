// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnusedCodeRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the UnusedCodeRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Rules.Semantic
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using ArchiMetrics.Analysis;
	using ArchiMetrics.Common;
	using ArchiMetrics.Common.CodeReview;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.FindSymbols;

	internal abstract class UnusedCodeRule : SemanticEvaluationBase
	{
		public override string Title
		{
			get
			{
				return "Unused " + EvaluatedKind.ToString().ToTitleCase();
			}
		}

		public override string Suggestion
		{
			get { return "Remove unused code."; }
		}

		public override ImpactLevel ImpactLevel
		{
			get
			{
				return ImpactLevel.Member;
			}
		}

		public override CodeQuality Quality
		{
			get
			{
				return CodeQuality.NeedsCleanup;
			}
		}

		public override QualityAttribute QualityAttribute
		{
			get
			{
				return QualityAttribute.CodeQuality;
			}
		}

		protected override async Task<EvaluationResult> EvaluateImpl(SyntaxNode node, SemanticModel semanticModel, Solution solution)
		{
			var symbol = semanticModel.GetDeclaredSymbol(node);
			var callers = await solution.FindReferences(symbol).ConfigureAwait(false);

			if (!callers.Locations.Any(IsNotAssignment))
			{
				return new EvaluationResult
					   {
						   Snippet = node.ToFullString()
					   };
			}

			return null;
		}

		private bool IsNotAssignment(Location location)
		{
			if (!location.IsInSource)
			{
				return false;
			}

			var token = location.SourceTree.GetRoot().FindToken(location.SourceSpan.Start);
			var assignmentSyntax = GetAssignmentSyntax(token.Parent);
			if (assignmentSyntax == null)
			{
				return true;
			}

			return false;
		}

		private SyntaxNode GetAssignmentSyntax(SyntaxNode node)
		{
			if (node == null)
			{
				return null;
			}

			if (node.IsKind(SyntaxKind.EqualsExpression))
			{
				return node;
			}

			return GetAssignmentSyntax(node.Parent);
		}
	}
}
