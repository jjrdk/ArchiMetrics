namespace ArchiMetrics.UI
{
	using System.Reactive.Concurrency;

	internal static class Schedulers
	{
		public static IScheduler Taskpool { get; set; }

		public static IScheduler Dispatcher { get; set; }
	}
}