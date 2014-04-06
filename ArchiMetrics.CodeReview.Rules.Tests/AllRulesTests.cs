// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AllRulesTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the AllRulesTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Rules.Tests
{
	using NUnit.Framework;

	public class AllRulesTests
	{
		[Test]
		public void CanGetEnumeratinoOfCodeReviewTypes()
		{
			CollectionAssert.IsNotEmpty(AllRules.GetRules());
		}
	}
}
