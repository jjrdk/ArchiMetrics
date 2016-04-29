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
    using System.Threading.Tasks;
    using ArchiMetrics.Analysis.Metrics;
    using Common;
    using Common.Metrics;
    using metrics;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.MSBuild;
    using Xunit;

    public class MetricsCalculationPerformanceTest
    {
        private readonly ProjectMetricsCalculator _calculator;

        public MetricsCalculationPerformanceTest()
        {
            _calculator = new ProjectMetricsCalculator(new CodeMetricsCalculator(new TypeDocumentationFactory(), new MemberDocumentationFactory()));
        }

        [Fact]
        public async Task MeasureSolutionAnalysisPerformance()
        {
            using (var workspace = MSBuildWorkspace.Create())
            {
                var path = @"..\..\..\..\archimetrics.sln".GetLowerCaseFullPath();
                var solution = await workspace.OpenSolutionAsync(path).ConfigureAwait(false);
                var metrics = new metrics.Metrics();
                var timer = metrics.Timer(GetType(), "test", TimeUnit.Seconds, TimeUnit.Seconds);
                for (var i = 0; i < 5; i++)
                {
                    var amount = timer.Time(() => PerformReview(solution).Result);
                }

                Assert.True(timer.Mean < 90.0);
            }
        }

        [Fact]
        public async Task MeasureProjectAnalysisPerformance()
        {
            using (var workspace = MSBuildWorkspace.Create())
            {
                var path = @"..\..\..\..\src\ArchiMetrics.Analysis\ArchiMetrics.Analysis.csproj".GetLowerCaseFullPath();
                var project = await workspace.OpenProjectAsync(path).ConfigureAwait(false);
                var metrics = new metrics.Metrics();
                var timer = metrics.Timer(GetType(), "test", TimeUnit.Seconds, TimeUnit.Seconds);
                for (var i = 0; i < 5; i++)
                {
                    var amount = timer.Time(() => PerformReview(project).Result);
                }

                Assert.True(timer.Mean < 90.0);
            }
        }

        private async Task<int> PerformReview(Solution solution)
        {
            var results = await _calculator.Calculate(solution).ConfigureAwait(false);
            var amount = results.AsArray();
            return amount.Length;
        }

        private async Task<IProjectMetric> PerformReview(Project project)
        {
            var results = await _calculator.Calculate(project, null).ConfigureAwait(false);

            return results;
        }
    }
}