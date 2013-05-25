namespace ArchiMeter.CodeReview.Rules
{
	using Common;
	using Roslyn.Compilers.CSharp;

	internal class PropertyNameSpellingRule : NameSpellingRuleBase
	{
		public PropertyNameSpellingRule(ISpellChecker speller, IKnownWordList knownWordList)
			: base(speller, knownWordList)
		{
		}

		public override SyntaxKind EvaluatedKind
		{
			get { return SyntaxKind.PropertyDeclaration; }
		}

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var propertyDeclaration = (MethodDeclarationSyntax)node;
			var propertyName = propertyDeclaration.Identifier.ValueText;

			var correct = IsSpelledCorrectly(propertyName);
			if (!correct)
			{
				return new EvaluationResult
					   {
						   Comment = "Possible spelling error",
						   Quality = CodeQuality.NeedsReview,
						   ImpactLevel = ImpactLevel.Node,
						   QualityAttribute = QualityAttribute.Conformance,
						   Snippet = propertyName,
						   ErrorCount = 1
					   };
			}

			return null;
		}
	}
}