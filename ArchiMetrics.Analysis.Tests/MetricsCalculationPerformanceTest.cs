// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MetricsCalculationPerformanceTest.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the MetricsCalculationPerformanceTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Tests
{
	using System;
	using System.Linq;
	using System.Threading.Tasks;
	using ArchiMetrics.Analysis.Metrics;
	using ArchiMetrics.Common;
	using metrics;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.MSBuild;
	using NUnit.Framework;

	public class MetricsCalculationPerformanceTest
	{
		private ProjectMetricsCalculator _calculator;

		[SetUp]
		public void Setup()
		{
			_calculator = new ProjectMetricsCalculator(new CodeMetricsCalculator());
		}

		[Test]
		public async Task MeasurePerformance()
		{
			using (var workspace = MSBuildWorkspace.Create())
			{
				var path = @"..\..\..\archimetrics.sln".GetLowerCaseFullPath();
				var solution = await workspace.OpenSolutionAsync(path)
										 .ConfigureAwait(false);
				var timer = metrics.Metrics.Timer(GetType(), "test", TimeUnit.Seconds, TimeUnit.Seconds);
				for (var i = 0; i < 5; i++)
				{
					Console.WriteLine(i.ToString());

					var amount = timer.Time(() => PerformReview(solution).Result);
				}

				Console.WriteLine("Min " + TimeSpan.FromSeconds(timer.Min));
				Console.WriteLine("Max " + TimeSpan.FromSeconds(timer.Max));
				Console.WriteLine("Mean " + TimeSpan.FromSeconds(timer.Mean));
				Assert.Less(timer.Mean, 90.0);
			}
		}

		private async Task<int> PerformReview(Solution solution)
		{
			var results = await _calculator.Calculate(solution).ConfigureAwait(false);
			var amount = results.ToArray();
			return amount.Length;
		}
	}
}