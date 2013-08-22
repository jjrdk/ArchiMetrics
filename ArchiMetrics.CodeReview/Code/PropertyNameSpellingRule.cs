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

namespace ArchiMetrics.CodeReview.Code
{
	using ArchiMetrics.Common;
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

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var propertyDeclaration = (PropertyDeclarationSyntax)node;
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
