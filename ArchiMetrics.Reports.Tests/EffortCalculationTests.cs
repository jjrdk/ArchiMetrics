// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EffortCalculationTests.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the EffortCalculationTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Reports.Tests
{
	using System;
	using System.Linq;
	using NUnit.Framework;

	class EffortCalculationTests
	{
		[Test]
		public void CalculatesExpectedEffort()
		{
			var totalEffort = GetEffort(4);

			Assert.AreEqual(18.75, totalEffort);
		}

		private static double GetEffort(int violations)
		{
			var baseEffort = 10;// metrics.Sum(m => m.GetEffort().TotalSeconds);
			return Enumerable.Range(0, violations).Aggregate(0.0, (d, i) => d + (baseEffort * Math.Pow(0.5, i)));
		}

	}
}
