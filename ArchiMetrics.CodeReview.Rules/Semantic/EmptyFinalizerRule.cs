namespace ArchiMetrics.CodeReview.Rules.Semantic
{
	using System.Diagnostics;
	using System.Linq;
	using System.Runtime.InteropServices.WindowsRuntime;
	using System.Threading.Tasks;
	using ArchiMetrics.Common.CodeReview;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;

	internal class EmptyFinalizerRule : SemanticEvaluationBase
	{
		public override string ID
		{
			get
			{
				return "CA1821";
			}
		}

		public override string Title
		{
			get
			{
				return "Empty Finalizer Detected";
			}
		}

		public override string Suggestion
		{
			get
			{
				return "Finalizer should call dispose method.";
			}
		}

		public override CodeQuality Quality
		{
			get
			{
				return CodeQuality.NeedsReview;
			}
		}

		public override QualityAttribute QualityAttribute
		{
			get
			{
				return QualityAttribute.Performance;
			}
		}

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
				return SyntaxKind.DestructorDeclaration;
			}
		}

		protected override Task<EvaluationResult> EvaluateImpl(SyntaxNode node, SemanticModel semanticModel, Solution solution)
		{
			if (IsEmptyFinalizer(node, semanticModel))
			{
				var result = new EvaluationResult
								 {
									 Snippet = node.ToFullString()
								 };

				return Task.FromResult(result);
			}

			return Task.FromResult<EvaluationResult>(null);
		}
		
		private bool IsEmptyFinalizer(SyntaxNode node, SemanticModel model)
		{
			// NOTE: FxCop only checks if there is any method call within a given destructor to decide an empty finalizer.
			// Here in order to minimize false negatives, we conservatively treat it as non-empty finalizer if its body contains any statements.
			// But, still conditional methods like Debug.Fail() will be considered as being empty as FxCop currently does.

			return node.DescendantNodes().OfType<StatementSyntax>()
				.Where(n => !n.IsKind(SyntaxKind.Block) && !n.IsKind(SyntaxKind.EmptyStatement))
				.Select(exp => exp as ExpressionStatementSyntax)
				.All(method => method != null && HasConditionalAttribute(method.Expression, model));
		}

		private bool HasConditionalAttribute(SyntaxNode root, SemanticModel model)
		{
			var node = root as InvocationExpressionSyntax;
			if (node != null)
			{
				var exp = node.Expression as MemberAccessExpressionSyntax;
				if (exp != null)
				{
					var symbolInfo = model.GetSymbolInfo(exp);
					var symbol = symbolInfo.Symbol;
					if (symbol != null && symbol.GetAttributes().Any(n => n.AttributeClass.MetadataName.Equals("ConditionalAttribute"))) //System.Diagnostics.ConditionalAttribute
					{
						return true;
					}
				}
			}

			return false;
		}
	}
}