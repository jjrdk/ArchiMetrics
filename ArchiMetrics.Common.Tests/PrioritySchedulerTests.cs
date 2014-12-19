// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PrioritySchedulerTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
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
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.CodeAnalysis;
	using System.Threading;
	using System.Threading.Tasks;
	using NUnit.Framework;

	[SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Disposed in teardown.")]
	public class PrioritySchedulerTests
	{
		private TestPriorityScheduler _scheduler;

		[SetUp]
		public void Setup()
		{
			_scheduler = new TestPriorityScheduler(ThreadPriority.AboveNormal);
		}

		[TearDown]
		public void Teardown()
		{
			_scheduler.Dispose();
		}

		[Test]
		public void MaximumConcurrencyLevelEqualsProcessorCount()
		{
			var processorCount = Environment.ProcessorCount;

			Assert.AreEqual(processorCount, _scheduler.MaximumConcurrencyLevel);
		}

		[Test]
		public async Task WhenExecutingTaskOnSchedulerThenExecutes()
		{
			var value = await GetValue(_scheduler);

			Assert.AreEqual(100, value);
		}

		[Test]
		public void WhenDisposingThenDoesNotThrow()
		{
			Assert.DoesNotThrow(() => _scheduler.Dispose());
		}

		[Test]
		public void WhenGettingScheduledTasksThenIsNotNull()
		{
			Assert.NotNull(_scheduler.ScheduledTasks());
		}

		[Test]
		public void WhenCheckingIfCanExecuteInlineThenReturnsFalse()
		{
			Assert.False(_scheduler.CanExecuteInline());
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

		private class TestPriorityScheduler : PriorityScheduler
		{
			public TestPriorityScheduler(ThreadPriority priority)
				: base(priority)
			{
			}

			public IEnumerable<Task> ScheduledTasks()
			{
				return GetScheduledTasks();
			}

			public bool CanExecuteInline()
			{
				return TryExecuteTaskInline(Task.FromResult(1), false);
			}
		}
	}
}
