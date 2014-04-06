namespace ArchiMetrics.UI.Support
{
	using System;
	using System.Linq;
	using ArchiMetrics.Common;

	internal class AggregateDisposable : IDisposable
	{
		private readonly IDisposable[] _disposables;

		public AggregateDisposable(params IDisposable[] disposables)
		{
			_disposables = disposables.WhereNot(x => x == null).ToArray();
		}

		~AggregateDisposable()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
		}

		private void Dispose(bool isDisposing)
		{
			if (isDisposing)
			{
				foreach (var disposable in _disposables)
				{
					disposable.DisposeNotNull();
				}
			}
		}
	}
}
