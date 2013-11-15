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
	using System.Threading;
	using System.Threading.Tasks;

	public class PriorityScheduler : TaskScheduler, IDisposable
	{
		private static readonly PriorityScheduler AboveNormalScheduler = new PriorityScheduler(ThreadPriority.AboveNormal);
		private static readonly PriorityScheduler BelowNormalScheduler = new PriorityScheduler(ThreadPriority.BelowNormal);
		private static readonly PriorityScheduler LowestScheduler = new PriorityScheduler(ThreadPriority.Lowest);

		private readonly int _maximumConcurrencyLevel = Math.Max(1, Environment.ProcessorCount);
		private readonly BlockingCollection<Task> _tasks = new BlockingCollection<Task>();
		private readonly ThreadPriority _priority;
		private Thread[] _threads;

		private PriorityScheduler(ThreadPriority priority)
		{
			_priority = priority;
		}

		~PriorityScheduler()
		{
			Dispose(false);
		}

		public static PriorityScheduler AboveNormal
		{
			get
			{
				return AboveNormalScheduler;
			}
		}

		public static PriorityScheduler BelowNormal
		{
			get
			{
				return BelowNormalScheduler;
			}
		}

		public static PriorityScheduler Lowest
		{
			get
			{
				return LowestScheduler;
			}
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
			}
		}

		protected override IEnumerable<Task> GetScheduledTasks()
		{
			return _tasks;
		}

		protected override void QueueTask(Task task)
		{
			_tasks.Add(task);

			if (_threads == null)
			{
				_threads = new Thread[_maximumConcurrencyLevel];
				for (int i = 0; i < _threads.Length; i++)
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
									  Priority = _priority,
									  IsBackground = true
								  };
					_threads[i].Start();
				}
			}
		}

		protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
		{
			return false; // we might not want to execute task that should schedule as high or low priority inline
		}
	}
}