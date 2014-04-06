namespace ArchiMetrics.Common.Metrics
{
	public interface ICodeMetric
	{
		int LinesOfCode { get; }
		
		double MaintainabilityIndex { get; }
		
		int CyclomaticComplexity { get; }
		
		string Name { get; }
	}
}