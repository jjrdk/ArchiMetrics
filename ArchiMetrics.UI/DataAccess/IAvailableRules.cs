namespace ArchiMetrics.UI.DataAccess
{
	using System;
	using System.Collections.Generic;
	using System.Collections.Specialized;
	using ArchiMetrics.Common.CodeReview;

	public interface IAvailableRules : IEnumerable<IEvaluation>, INotifyCollectionChanged, IDisposable
	{
		IEnumerable<IAvailability> Availabilities { get; }
	}
}