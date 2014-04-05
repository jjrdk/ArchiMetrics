// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnreadValueRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
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
	using System.Threading;
	using System.Threading.Tasks;
	using ArchiMetrics.Common.CodeReview;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;
	using Microsoft.CodeAnalysis.FindSymbols;

	internal abstract class UnreadValueRule : SemanticEvaluationBase
	{
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
				.Select(x => SymbolFinder.FindReferencesAsync(x, solution, CancellationToken.None));
			var references = (await Task.WhenAll(referenceTasks))
				.SelectMany(x => x)
				.SelectMany(x => x.Locations)
				.Select(x => x.Location.SourceTree.GetRoot().FindToken(x.Location.SourceSpan.Start))
				.Select(x => x.Parent)
				.Where(x => x != null)
				.Select(x => x.Parent)
				.Where(IsNotAssignment)
				.ToArray();

			if (!references.Any())
			{
				return new EvaluationResult
					   {
						   Snippet = node.ToFullString()
					   };
			}

			return null;
		}

		private static bool IsNotAssignment(SyntaxNode syntax)
		{
			var expression = syntax as BinaryExpressionSyntax;
			if (expression != null)
			{
				return expression.IsKind(SyntaxKind.EqualsExpression);
			}

			return true;
		}
	}
}