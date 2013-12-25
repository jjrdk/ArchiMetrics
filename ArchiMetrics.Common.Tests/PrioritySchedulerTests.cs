// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PrioritySchedulerTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the PrioritySchedulerTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Common.Tests
{
	using System.Threading;
	using System.Threading.Tasks;
	using NUnit.Framework;

	public class PrioritySchedulerTests
	{
		[Test]
		public void AboveNormalPrioritySchedulerIsNotNull()
		{
			Assert.NotNull(PriorityScheduler.AboveNormal);
		}

		[Test]
		public void BelowNormalPrioritySchedulerIsNotNull()
		{
			Assert.NotNull(PriorityScheduler.BelowNormal);
		}

		[Test]
		public void LowestNormalPrioritySchedulerIsNotNull()
		{
			Assert.NotNull(PriorityScheduler.Lowest);
		}

		[Test]
		public async Task WhenExecutingTaskOnAboveNormalSchedulerThenExecutes()
		{
			var value = await GetValue(PriorityScheduler.AboveNormal);

			Assert.AreEqual(100, value);
		}

		[Test]
		public async Task WhenExecutingTaskOnBelowNormalSchedulerThenExecutes()
		{
			var value = await GetValue(PriorityScheduler.BelowNormal);

			Assert.AreEqual(100, value);
		}

		[Test]
		public async Task WhenExecutingTaskOnLowestSchedulerThenExecutes()
		{
			var value = await GetValue(PriorityScheduler.Lowest);

			Assert.AreEqual(100, value);
		}

		private static async Task<int> GetValue(TaskScheduler scheduler)
		{
			return await Task.Factory.StartNew(
				() =>
				{
					Thread.Sleep(100);
					return 100;
				},
				CancellationToken.None,
				TaskCreationOptions.None,
				scheduler);
		}
	}
}
