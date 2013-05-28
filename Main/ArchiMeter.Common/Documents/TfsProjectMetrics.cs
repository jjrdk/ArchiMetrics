namespace ArchiMeter.Common.Documents
{
	public class TfsProjectMetrics : ProjectDocument
	{
		public int LinesOfCode { get; set; }

		public double MaintainabilityIndex { get; set; }

		public int CyclomaticComplexity { get; set; }
	}
}