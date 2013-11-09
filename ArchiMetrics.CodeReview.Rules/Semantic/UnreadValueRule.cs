// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnreadValueRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
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
	using ArchiMetrics.Common.CodeReview;
	using Roslyn.Compilers.Common;
	using Roslyn.Compilers.CSharp;
	using Roslyn.Services;

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

		protected abstract IEnumerable<ISymbol> GetSymbols(SyntaxNode node, ISemanticModel semanticModel);

		protected override EvaluationResult EvaluateImpl(SyntaxNode node, ISemanticModel semanticModel, ISolution solution)
		{
			var references = GetSymbols(node, semanticModel)
				.SelectMany(x => SymbolExtensions.FindReferences(x, solution, CancellationToken.None))
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

		private static bool IsNotAssignment(CommonSyntaxNode syntax)
		{
			var expression = syntax as BinaryExpressionSyntax;
			if (expression != null)
			{
				return expression.Kind != SyntaxKind.AssignExpression;
			}

			return true;
		}
	}
}