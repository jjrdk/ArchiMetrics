// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefinedRulesTests.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the DefinedRulesTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMeter.CodeReview.Tests
{
	using NUnit.Framework;

	public class DefinedRulesTests
	{
		[Test]
		public void CanGetDefinedRules()
		{
			Assert.IsNotEmpty(DefinedRules.Default);
		}
	}
}
