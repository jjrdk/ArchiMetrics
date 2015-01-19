namespace ArchiMetrics.Common.Metrics
{
	public interface IDocumentedMetric : ICodeMetric
	{
		IDocumentation Documentation { get; }
	}
}