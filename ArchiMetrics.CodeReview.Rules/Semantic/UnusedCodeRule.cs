namespace ArchiMetrics.CodeReview.Rules.Semantic
{
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using ArchiMetrics.Common;
	using ArchiMetrics.Common.CodeReview;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;
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
			var callers = await SymbolFinder.FindReferencesAsync(symbol, solution, CancellationToken.None);

			if (!callers.Any(IsNotAssignment))
			{
				return new EvaluationResult
					   {
						   Snippet = node.ToFullString()
					   };
			}

			return null;
		}

		private bool IsNotAssignment(ReferencedSymbol referencedSymbol)
		{
			var locations = referencedSymbol.Locations.ToArray();
			return locations.Any(x => !x.Location.IsInSource)
				   || locations.All(x => IsNotAssignment(x.Location));
		}

		private bool IsNotAssignment(Location location)
		{
			var token = location.SourceTree.GetRoot()
				.FindToken(location.SourceSpan.Start);
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
