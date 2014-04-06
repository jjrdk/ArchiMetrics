namespace ArchiMetrics.CodeReview.Rules.Code
{
	using ArchiMetrics.Common.CodeReview;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;

	internal class TypeMustBeDeclaredInNamespaceRule : CodeEvaluationBase
	{
		public override string Suggestion
		{
			get
			{
				return "Move type declaration inside namespace.";
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
				return "Declare Types Inside Namespace.";
			}
		}

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var ns = FindNamespaceParent(node);
			if (ns == null)
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