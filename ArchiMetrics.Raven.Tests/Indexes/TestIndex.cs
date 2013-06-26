namespace ArchiMetrics.Raven.Tests.Indexes
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq.Expressions;

	public interface ITestIndex<TDocument, TReduce>
	{
		Expression<Func<IEnumerable<TDocument>, IEnumerable>> GetMap();

		Expression<Func<IEnumerable<TReduce>, IEnumerable>> GetReduce();
	}
}
