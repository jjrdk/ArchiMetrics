namespace ArchiMeter.Common.Documents
{
	public class TfsProjectMetrics
	{
		public string ProjectName { get; set; }

		public int LinesOfCode { get; set; }

		public double MaintainabilityIndex { get; set; }

		public int CyclomaticComplexity { get; set; }
	}
}