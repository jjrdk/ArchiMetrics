namespace ArchiMetrics.CodeReview.Rules.Semantic
{
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using ArchiMetrics.Common;
	using ArchiMetrics.Common.CodeReview;
	using Microsoft.CodeAnalysis;
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
			var callers = await SymbolFinder.FindCallersAsync(symbol, solution, CancellationToken.None);

			if (!callers.Any())
			{
				return new EvaluationResult
					   {
						   Snippet = node.ToFullString()
					   };
			}

			return null;
		}
	}
}
