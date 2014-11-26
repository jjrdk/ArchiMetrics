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
	using ArchiMetrics.Common.Metrics;
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
		public async Task MeasureSolutionAnalysisPerformance()
		{
			using (var workspace = MSBuildWorkspace.Create())
			{
				var path = @"..\..\..\archimetrics.sln".GetLowerCaseFullPath();
				var solution = await workspace.OpenSolutionAsync(path).ConfigureAwait(false);
				var metrics = new metrics.Metrics();
				var timer = metrics.Timer(GetType(), "test", TimeUnit.Seconds, TimeUnit.Seconds);
				for (var i = 0; i < 5; i++)
				{
					var amount = timer.Time(() => PerformReview(solution).Result);
				}

				Assert.Less(timer.Mean, 90.0);
			}
		}

		[Test]
		public async Task MeasureProjectAnalysisPerformance()
		{
			using (var workspace = MSBuildWorkspace.Create())
			{
				var path = @"..\..\..\ArchiMetrics.Common\ArchiMetrics.Common.csproj".GetLowerCaseFullPath();
				var project = await workspace.OpenProjectAsync(path).ConfigureAwait(false);
				var metrics = new metrics.Metrics();
				var timer = metrics.Timer(GetType(), "test", TimeUnit.Seconds, TimeUnit.Seconds);
				for (var i = 0; i < 5; i++)
				{
					var amount = timer.Time(() => PerformReview(project).Result);
				}

				Assert.Less(timer.Mean, 90.0);
			}
		}

		private async Task<int> PerformReview(Solution solution)
		{
			var results = await _calculator.Calculate(solution).ConfigureAwait(false);
			var amount = results.ToArray();
			return amount.Length;
		}

		private async Task<IProjectMetric> PerformReview(Project project)
		{
			var results = await _calculator.Calculate(project, null).ConfigureAwait(false);

			return results;
		}
	}
}