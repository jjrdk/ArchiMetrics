namespace ArchiMetrics.CodeReview.Rules.Code
{
	using System;
	using System.Linq;
	using ArchiMetrics.Common;
	using ArchiMetrics.Common.CodeReview;

	internal abstract class NameSpellingRuleBase : CodeEvaluationBase
	{
		private readonly ISpellChecker _speller;

		protected NameSpellingRuleBase(ISpellChecker speller)
		{
			_speller = speller;
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
				return QualityAttribute.Conformance;
			}
		}

		public override ImpactLevel ImpactLevel
		{
			get
			{
				return ImpactLevel.Node;
			}
		}

		protected bool IsSpelledCorrectly(string name)
		{
			return name.ToTitleCase()
				.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries)
				.Aggregate(true, (b, s) => b && _speller.Spell(s));
		}
	}
}
