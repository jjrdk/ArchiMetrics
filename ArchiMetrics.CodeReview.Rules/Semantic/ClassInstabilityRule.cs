// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClassInstabilityRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ClassInstabilityRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Rules.Semantic
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using ArchiMetrics.Common;
	using ArchiMetrics.Common.CodeReview;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;
	using Microsoft.CodeAnalysis.FindSymbols;

	internal class ClassInstabilityRule : SemanticEvaluationBase
	{
		public override ImpactLevel ImpactLevel
		{
			get
			{
				return ImpactLevel.Type;
			}
		}

		public override SyntaxKind EvaluatedKind
		{
			get
			{
				return SyntaxKind.ClassDeclaration;
			}
		}

		public override string Title
		{
			get
			{
				return "Unstable Class";
			}
		}

		public override string Suggestion
		{
			get
			{
				return "Refactor class dependencies.";
			}
		}

		public override CodeQuality Quality
		{
			get
			{
				return CodeQuality.NeedsRefactoring;
			}
		}

		public override QualityAttribute QualityAttribute
		{
			get
			{
				return QualityAttribute.Maintainability | QualityAttribute.Modifiability;
			}
		}

		protected override async Task<EvaluationResult> EvaluateImpl(SyntaxNode node, SemanticModel semanticModel, Solution solution)
		{
			var symbol = (ITypeSymbol)semanticModel.GetDeclaredSymbol(node);
			var efferent = GetReferencedTypes(node, symbol, semanticModel).ToArray();
			var awaitable = SymbolFinder.FindCallersAsync(symbol, solution, CancellationToken.None).ConfigureAwait(false);
			var callers = (await awaitable).ToArray();
			var testCallers = callers
				.Where(c => c.CallingSymbol.GetAttributes()
				.Any(x => x.AttributeClass.Name.IsKnownTestAttribute()))
				.ToArray();
			var afferent = callers.Except(testCallers)
				.Select(x => x.CallingSymbol.ContainingType)
				.DistinctBy(s => s.ToDisplayString())
				.ToArray();

			var efferentLength = (double)efferent.Length;
			var stability = efferentLength / (efferentLength + afferent.Length);
			if (stability >= 0.8)
			{
				return new EvaluationResult
				{
					ImpactLevel = ImpactLevel.Project,
					Quality = CodeQuality.NeedsReview,
					QualityAttribute = QualityAttribute.CodeQuality | QualityAttribute.Conformance,
					Snippet = node.ToFullString()
				};
			}

			return null;
		}

		private static IEnumerable<ITypeSymbol> GetReferencedTypes(SyntaxNode classDeclaration, ISymbol sourceSymbol, SemanticModel semanticModel)
		{
			var typeSyntaxes = classDeclaration.DescendantNodesAndSelf().OfType<TypeSyntax>();
			var commonSymbolInfos = typeSyntaxes.Select(x => semanticModel.GetSymbolInfo(x)).ToArray();
			var members = commonSymbolInfos
				.Select(x => x.Symbol)
				.Where(x => x != null)
				.Select(x =>
				{
					var typeSymbol = x as ITypeSymbol;
					return typeSymbol == null ? x.ContainingType : x;
				})
				.Cast<ITypeSymbol>()
				.WhereNotNull()
				.DistinctBy(x => x.ToDisplayString())
				.Where(x => x != sourceSymbol)
				.ToArray();

			return members;
		}
	}
}