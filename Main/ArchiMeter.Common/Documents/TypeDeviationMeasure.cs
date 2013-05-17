namespace ArchiMeter.Common.Documents
{
	public abstract class TypeDeviationMeasure
	{
		public string ProjectName { get; set; }

		public string NamespaceName { get; set; }

		public string TypeName { get; set; }

		public double Sigma { get; set; }
	}
}