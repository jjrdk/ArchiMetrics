namespace ArchiMetrics.Common.Documents
{
	public class MemberComplexityMaintainabilitySegment : DataSegment
	{
		public int CyclomaticComplexity { get; set; }

		public double MaintainabilityIndex { get; set; }
	}
}
