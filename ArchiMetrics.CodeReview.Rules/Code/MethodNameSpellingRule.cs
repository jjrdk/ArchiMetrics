namespace ArchiMetrics.CodeReview.Rules.Code
{
	using ArchiMetrics.Common.CodeReview;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;

	internal class MethodNameSpellingRule : NameSpellingRuleBase
	{
		public MethodNameSpellingRule(ISpellChecker speller)
			: base(speller)
		{
		}

		public override SyntaxKind EvaluatedKind
		{
			get { return SyntaxKind.MethodDeclaration; }
		}

		public override string Title
		{
			get
			{
				return "Method Name Spelling";
			}
		}

		public override string Suggestion
		{
			get
			{
				return "Check that the method name is spelled correctly. Consider adding exceptions to the dictionary.";
			}
		}

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var methodDeclaration = (MethodDeclarationSyntax)node;
			var methodName = methodDeclaration.Identifier.ValueText;

			var correct = IsSpelledCorrectly(methodName);
			if (!correct)
			{
				return new EvaluationResult
					   {
						   Quality = CodeQuality.NeedsReview, 
						   ImpactLevel = ImpactLevel.Node, 
						   QualityAttribute = QualityAttribute.Conformance, 
						   Snippet = methodName, 
						   ErrorCount = 1
					   };
			}

			return null;
		}
	}
}
