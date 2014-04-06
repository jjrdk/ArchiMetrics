namespace OpenCover.Framework
{
	using System;
	using System.Security.Principal;
	using System.Threading;
	using System.Threading.Tasks;
	using ArchiMetrics.Common;
	using OpenCover.Framework.Communication;
	using OpenCover.Framework.Manager;
	using OpenCover.Framework.Persistance;
	using OpenCover.Framework.Utility;

	public class ProfilerManagerFactory : IAsyncFactory<IProfilerManager>
	{
		private readonly ICommunicationManager _communicationManager;
		private readonly IPersistance _persistance;
		private readonly IMemoryManager _memoryManager;
		private readonly ICommandLine _commandLine;

		public ProfilerManagerFactory(ICommunicationManager communicationManager,
			IPersistance persistance,
			IMemoryManager memoryManager,
			ICommandLine commandLine)
		{
			_communicationManager = communicationManager;
			_persistance = persistance;
			_memoryManager = memoryManager;
			_commandLine = commandLine;
		}

		public Task<IProfilerManager> Create(CancellationToken cancellationToken)
		{
			return Task.Factory.StartNew(
				new Func<IProfilerManager>(CreateProfilerManager),
				cancellationToken);
		}

		private IProfilerManager CreateProfilerManager()
		{
			IPerfCounters perfCounter = new NullPerfCounter();

			if (new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator))
			{
				perfCounter = new PerfCounters();
			}

			IProfilerManager profilerManager = new ProfilerManager(_communicationManager, _persistance, _memoryManager, _commandLine, perfCounter);
			return profilerManager;
		}

		~ProfilerManagerFactory()
		{
			Dispose(false);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
		}

		protected void Dispose(bool isDisposing)
		{
			if (isDisposing)
			{
				_memoryManager.Dispose();
			}

			// Dispose unmanaged resources.
		}
	}
}
