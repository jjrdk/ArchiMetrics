namespace ArchiMetrics.Common
{
	using System;

	public interface IProvider<out T> : IDisposable
	{
		T Get();
	}

	public interface IProvider<in TKey, out T> : IDisposable
	{
		T Get(TKey key);
	}

	public interface IProvider<in TKey1, in TKey2, out T> : IDisposable
	{
		T Get(TKey1 key1, TKey2 key2);
	}
}
