// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnreadValueRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the UnreadValueRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Rules.Semantic
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using ArchiMetrics.Common;
	using ArchiMetrics.Analysis;
	using ArchiMetrics.Common.CodeReview;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;

	internal abstract class UnreadValueRule : SemanticEvaluationBase
	{
		public override string ID
		{
			get
			{
				return "AMS0008";
			}
		}

		public override CodeQuality Quality
		{
			get { return CodeQuality.NeedsReview; }
		}

		public override QualityAttribute QualityAttribute
		{
			get { return QualityAttribute.CodeQuality | QualityAttribute.Maintainability; }
		}

		public override ImpactLevel ImpactLevel
		{
			get { return ImpactLevel.Type; }
		}

		protected abstract IEnumerable<ISymbol> GetSymbols(SyntaxNode node, SemanticModel semanticModel);

		protected override async Task<EvaluationResult> EvaluateImpl(SyntaxNode node, SemanticModel semanticModel, Solution solution)
		{
			var referenceTasks = GetSymbols(node, semanticModel)
				.Select(solution.FindReferences);
			var references = (await Task.WhenAll(referenceTasks).ConfigureAwait(false))
				.SelectMany(x => x.Locations)
				.Select(x => x.Location.SourceTree.GetRoot().FindToken(x.Location.SourceSpan.Start))
				.Select(x => x.Parent)
				.Where(x => x != null)
				.Select(x => new { Value = x, Parent = x.Parent })
				.Where(x => IsNotAssignment(x.Parent, x.Value))
				.AsArray();

			if (!references.Any())
			{
				return new EvaluationResult
					   {
						   Snippet = node.ToFullString()
					   };
			}

			return null;
		}

		private static bool IsNotAssignment(SyntaxNode syntax, SyntaxNode value)
		{
			if (syntax.IsKind(SyntaxKind.SimpleAssignmentExpression))
			{
				var binaryExpression = (AssignmentExpressionSyntax)syntax;
				return binaryExpression.Right == value;
			}

			var expression = syntax as EqualsValueClauseSyntax;
			if (expression != null)
			{
				return expression.Value == value;
			}

			return true;
		}
	}
}