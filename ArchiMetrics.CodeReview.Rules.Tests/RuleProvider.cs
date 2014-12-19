// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuleProvider.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the RuleProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Rules.Tests
{
	using System.Collections;
	using System.Linq;
	using ArchiMetrics.Common.CodeReview;
	using Moq;

	public static class RuleProvider
	{
		public static IEnumerable Rules
		{
			get
			{
				var spellChecker = new Mock<ISpellChecker>();
				spellChecker.Setup(x => x.Spell(It.IsAny<string>())).Returns(true);
				return AllRules.GetSyntaxRules(spellChecker.Object);
			}
		}
	}
}