namespace ArchiMetrics.CodeReview.Semantic
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using ArchiMetrics.CodeReview.Rules.Semantic;
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
			var efferent = GetReferencedTypes((ClassDeclarationSyntax)node, symbol, semanticModel).ToArray();
			var callers = (await SymbolFinder.FindCallersAsync(symbol, solution, CancellationToken.None)).ToArray();
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

		private static IEnumerable<ITypeSymbol> GetReferencedTypes(ClassDeclarationSyntax classDeclaration, ITypeSymbol sourceSymbol, SemanticModel semanticModel)
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
				.DistinctBy(x => x.ToDisplayString())
				.Where(x => x != sourceSymbol)
				.ToArray();

			return members;
		}

		// Ce / (Ce + Ca) - Ce = Efferent coupling (depends on), Ca = Afferent coupling (dependants)
		/*
		function sqr(x) { return x * x }
function dist2(v, w) { return sqr(v.x - w.x) + sqr(v.y - w.y) }
function distToSegmentSquared(p, v, w) {
  var l2 = dist2(v, w);
  if (l2 == 0) return dist2(p, v);
  var t = ((p.x - v.x) * (w.x - v.x) + (p.y - v.y) * (w.y - v.y)) / l2;
  if (t < 0) return dist2(p, v);
  if (t > 1) return dist2(p, w);
  return dist2(p, { x: v.x + t * (w.x - v.x),
                    y: v.y + t * (w.y - v.y) });
}
function distToSegment(p, v, w) { return Math.sqrt(distToSegmentSquared(p, v, w)); }
		*/
	}
}