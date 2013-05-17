namespace ArchiMeter.Common.Documents
{
	public class MemberComplexityDeviation : MemberDeviationMeasure
	{
		public int CyclomaticComplexity { get; set; }
	}

	public class MemberMaintainabilityDeviation : MemberDeviationMeasure
	{
		public double MaintainabilityIndex { get; set; }
	}
}