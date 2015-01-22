namespace ArchiMetrics.Common.Metrics
{
	public interface IDocumentation
	{
		string Summary { get; }

		string Returns { get; }

		string Code { get; }

		string Example { get; }

		string Remarks { get; }
	}
}