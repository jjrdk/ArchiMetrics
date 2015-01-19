namespace ArchiMetrics.Common.Metrics
{
	using System.Collections.Generic;

	public interface IDocumentation
	{
		string Summary { get; }

		string Returns { get; }

		string Code { get; }

		string Example { get; }

		string Remarks { get; }

		IEnumerable<ExceptionDescription> Exceptions { get; }
	}
}