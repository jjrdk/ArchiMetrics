// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyNameSpellingRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the PropertyNameSpellingRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Rules.Code
{
	using ArchiMetrics.Common.CodeReview;
	using Roslyn.Compilers.CSharp;

	internal class PropertyNameSpellingRule : NameSpellingRuleBase
	{
		public PropertyNameSpellingRule(ISpellChecker speller, IKnownPatterns knownPatterns)
			: base(speller, knownPatterns)
		{
		}

		public override SyntaxKind EvaluatedKind
		{
			get { return SyntaxKind.PropertyDeclaration; }
		}
		public override string Title
		{
			get
			{
				return "Property Name Spelling";
			}
		}
		public override string Suggestion
		{
			get
			{
				return "Check that the property name is spelled correctly. Consider adding exceptions to the dictionary.";
			}
		}

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var propertyDeclaration = (PropertyDeclarationSyntax)node;
			var propertyName = propertyDeclaration.Identifier.ValueText;

			var correct = IsSpelledCorrectly(propertyName);
			if (!correct)
			{
				return new EvaluationResult
					   {
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
