// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuleEvaluationPerformanceTest.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the RuleEvaluationPerformanceTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Rules.Tests
{
	using System;
	using System.IO;
	using System.Linq;
	using System.Threading.Tasks;
	using ArchiMetrics.Analysis;
	using ArchiMetrics.Common;
	using ArchiMetrics.Common.CodeReview;
	using metrics;
	using Microsoft.CodeAnalysis.MSBuild;
	using Moq;
	using NUnit.Framework;

	public class RuleEvaluationPerformanceTest
	{
		private NodeReviewer _reviewer;

		[SetUp]
		public void Setup()
		{
			var spellChecker = new Mock<ISpellChecker>();
			spellChecker.Setup(x => x.Spell(It.IsAny<string>())).Returns(true);

			_reviewer = new NodeReviewer(AllRules.GetSyntaxRules(spellChecker.Object).AsArray(), AllRules.GetSymbolRules());
		}

		[Test, Ignore("Run manually")]
		public void MeasurePerformance()
		{
			var metrics = new Metrics();
			var timer = metrics.Timer(GetType(), "test", TimeUnit.Seconds, TimeUnit.Seconds);
			for (var i = 0; i < 10; i++)
			{
				var amount = timer.Time(() => PerformReview().Result);
			}

			Assert.Less(timer.Mean, 90.0);
		}

		private async Task<int> PerformReview()
		{
			using (var workspace = MSBuildWorkspace.Create())
			{
				var path = @"..\..\..\archimetrics.sln".GetLowerCaseFullPath();
				var solution = await workspace.OpenSolutionAsync(path).ConfigureAwait(false);
				var results = await _reviewer.Inspect(solution).ConfigureAwait(false);
				var amount = results.AsArray();
				return amount.Length;
			}
		}
	}
}