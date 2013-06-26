namespace ArchiMetrics.Common.Documents
{
	public class MemberDeviationMeasure : ProjectDocument
	{
		public string NamespaceName { get; set; }

		public string TypeName { get; set; }

		public string MemberName { get; set; }

		public double Sigma { get; set; }
	}
}
