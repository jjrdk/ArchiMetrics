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

	public static class RuleProvider
	{
		public static IEnumerable Rules
		{
			get
			{
				return AllRules.GetRules()
					.Where(x => x.GetConstructors().Any(c => c.GetParameters().Length == 0));
			}
		}
	}
}