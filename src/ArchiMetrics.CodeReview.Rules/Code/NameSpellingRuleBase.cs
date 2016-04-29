// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NameSpellingRuleBase.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the NameSpellingRuleBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Rules.Code
{
	using System;
	using System.Linq;
	using Analysis.Common;
	using Analysis.Common.CodeReview;

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
