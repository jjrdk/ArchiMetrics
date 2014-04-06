// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BranchModelRuleTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the BranchModelRuleTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Tests.Validation
{
	using System.Linq;
	using System.Threading.Tasks;
	using ArchiMetrics.Analysis.Validation;
	using ArchiMetrics.Common.CodeReview;
	using ArchiMetrics.Common.Structure;
	using NUnit.Framework;

	public sealed class BranchModelRuleTests
	{
		private BranchModelRuleTests()
		{
		}

		[TestFixture]
		public class GivenABranchModelRule
		{
			[Test]
			public async Task WhenTreeContainsBranchThenIsPassed()
			{
				var rule = new BranchModelRule(new ModelNode("leaf", "type", CodeQuality.Good, 0, 0, 0));
				var tree = new ModelNode("node", "type", CodeQuality.Good, 0, 0, 0, new[] { new ModelNode("leaf", "type", CodeQuality.Good, 0, 0, 0) });

				var result = await rule.Validate(tree);

				Assert.IsTrue(result.All(x => x.Passed));
			}

			[Test]
			public async Task WhenTreeContainsComplexBranchThenIsPassed()
			{
				var rule = new BranchModelRule(new ModelNode("child", "type", CodeQuality.Good, 0, 0, 0, new[] { new ModelNode("leaf", "type", CodeQuality.Good, 0, 0, 0) }));
				var tree = new ModelNode("node", "type", CodeQuality.Good, 0, 0, 0, new[] { new ModelNode("child", "type", CodeQuality.Good, 0, 0, 0, new[] { new ModelNode("leaf", "type", CodeQuality.Good, 0, 0, 0) }) });

				var result = await rule.Validate(tree);

				Assert.IsTrue(result.All(x => x.Passed));
			}

			[Test]
			public async Task WhenTreeDoesNotContainBranchThenIsNotPassed()
			{
				var rule = new BranchModelRule(new ModelNode("cheese", "type", CodeQuality.Good, 0, 0, 0));
				var tree = new ModelNode("node", "type", CodeQuality.Good, 0, 0, 0, new[] { new ModelNode("child", "type", CodeQuality.Good, 0, 0, 0, new[] { new ModelNode("leaf", "type", CodeQuality.Good, 0, 0, 0) }) });

				var result = await rule.Validate(tree);

				Assert.IsFalse(result.Any(x => x.Passed));
			}
		}
	}
}
