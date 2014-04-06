namespace ArchiMetrics.Common
{
	using System;
	using System.Threading;
	using System.Threading.Tasks;

	public interface IAsyncFactory<T> : IDisposable
	{
		Task<T> Create(CancellationToken cancellationToken);
	}

	public interface IAsyncFactory<in TParameter, T> : IDisposable
	{
		Task<T> Create(TParameter parameter, CancellationToken cancellationToken);
	}
}