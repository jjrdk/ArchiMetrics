namespace ArchiMeter.Common
{
	using System;
	using System.Collections.Generic;
	using System.Linq.Expressions;
	using System.Threading.Tasks;

	public interface IAsyncReadOnlyRepository<T> : IDisposable
	{
		Task<IEnumerable<T>> Query(Expression<Func<T, bool>> query);
	}
}