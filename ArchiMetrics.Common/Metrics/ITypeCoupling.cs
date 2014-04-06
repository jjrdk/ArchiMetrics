namespace ArchiMetrics.Common.Metrics
{
	using System;
	using System.Collections.Generic;

	public interface ITypeCoupling : IComparable<ITypeCoupling>, ITypeDefinition
	{
		IEnumerable<string> UsedMethods { get; }

		IEnumerable<string> UsedProperties { get; }

		IEnumerable<string> UsedEvents { get; }
	}
}