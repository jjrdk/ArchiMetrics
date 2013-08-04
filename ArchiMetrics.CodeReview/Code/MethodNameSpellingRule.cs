// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MethodNameSpellingRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the MethodNameSpellingRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Code
{
	using ArchiMetrics.Common;
	using Roslyn.Compilers.CSharp;

	internal class MethodNameSpellingRule : NameSpellingRuleBase
	{
		public MethodNameSpellingRule(ISpellChecker speller, IKnownWordList knownWordList)
			: base(speller, knownWordList)
		{
		}

		public override SyntaxKind EvaluatedKind
		{
			get { return SyntaxKind.MethodDeclaration; }
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
						   Comment = "Possible spelling error", 
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
