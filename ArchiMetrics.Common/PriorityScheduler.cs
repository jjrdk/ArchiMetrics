// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PriorityScheduler.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the PriorityScheduler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Common
{
	using System;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;

	public class PriorityScheduler : TaskScheduler, IDisposable
	{
		private readonly int _maximumConcurrencyLevel = Math.Max(1, Environment.ProcessorCount);
		private readonly double _shutdownTimeout;
		private readonly BlockingCollection<Task> _tasks = new BlockingCollection<Task>();
		private readonly Thread[] _threads;

		public PriorityScheduler(ThreadPriority priority)
			: this(priority, Environment.ProcessorCount, 100)
		{
		}

		public PriorityScheduler(ThreadPriority priority, int maxConcurrencyLevel, double shutdownTimeout)
		{
			_maximumConcurrencyLevel = maxConcurrencyLevel;
			_shutdownTimeout = shutdownTimeout;
			_threads = new Thread[_maximumConcurrencyLevel];
			for (var i = 0; i < _threads.Length; i++)
			{
				_threads[i] = new Thread(
					() =>
					{
						foreach (var t in _tasks.GetConsumingEnumerable())
						{
							TryExecuteTask(t);
						}
					})
				{
					Name = "PriorityScheduler: " + i,
					Priority = priority,
					IsBackground = true
				};
				_threads[i].Start();
			}
		}

		~PriorityScheduler()
		{
			Dispose(false);
		}

		public override int MaximumConcurrencyLevel
		{
			get { return _maximumConcurrencyLevel; }
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				_tasks.Dispose();
				foreach (var thread in _threads.Where(thread => !thread.Join(TimeSpan.FromMilliseconds(_shutdownTimeout))))
				{
					thread.Interrupt();
				}
			}
		}

		protected override IEnumerable<Task> GetScheduledTasks()
		{
			return _tasks;
		}

		protected override void QueueTask(Task task)
		{
			_tasks.Add(task);
		}

		protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
		{
			return false; // we might not want to execute task that should schedule as high or low priority inline
		}
	}
}