// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MethodNameSpellingRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the MethodNameSpellingRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

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

		public override string ID
		{
			get
			{
				return "AM0024";
			}
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
