// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommentLanguageRuleBase.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the CommentLanguageRuleBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Rules.Trivia
{
	using System.Linq;
	using System.Text.RegularExpressions;
	using ArchiMetrics.Common.CodeReview;
	using Roslyn.Compilers.CSharp;

	internal abstract class CommentLanguageRuleBase : TriviaEvaluationBase
	{
		private static readonly Regex StrippedRegex = new Regex(@"[""'*©®º()!%\[\]{}/]+", RegexOptions.Compiled);
		private static readonly Regex NumberRegex = new Regex("[1-9]+", RegexOptions.Compiled);
		private static readonly Regex XmlRegex = new Regex("<.+?>", RegexOptions.Compiled);
		private readonly IKnownPatterns _knownPatterns;
		private readonly ISpellChecker _spellChecker;

		public override string Title
		{
			get
			{
				return "Suspicious Language Comment";
			}
		}

		public override string Suggestion
		{
			get
			{
				return "Check spelling of comment.";
			}
		}

		protected CommentLanguageRuleBase(ISpellChecker spellChecker, IKnownPatterns knownPatterns)
		{
			_spellChecker = spellChecker;
			_knownPatterns = knownPatterns;
		}

		protected override EvaluationResult EvaluateImpl(SyntaxTrivia node)
		{
			var trimmed = StrippedRegex.Replace(node.ToFullString(), string.Empty).Trim();
			var commentWords = RemoveXml(trimmed)
				.Split(' ')
				.Select(RemoveXml)
				.Select(s => s.TrimEnd('.', ','))
				.Where(IsNotNumber)
				.ToArray();
			var errorCount = commentWords.Aggregate(0, (i, s) => i + ((_knownPatterns.IsExempt(s) || _spellChecker.Spell(s)) ? 0 : 1));
			if (errorCount >= 0.50 * commentWords.Length)
			{
				return new EvaluationResult
						   {
							   Comment = "Suspicious language comment",
							   ErrorCount = 1,
							   ImpactLevel = ImpactLevel.Member,
							   Quality = CodeQuality.NeedsReview,
							   QualityAttribute = QualityAttribute.Maintainability | QualityAttribute.Conformance,
							   Snippet = node.ToFullString()
						   };
			}

			return null;
		}

		private bool IsNotNumber(string input)
		{
			return !NumberRegex.IsMatch(input);
		}

		private string RemoveXml(string input)
		{
			return XmlRegex.Replace(input, string.Empty);
		}
	}
}
