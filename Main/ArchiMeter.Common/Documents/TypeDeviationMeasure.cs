namespace ArchiMeter.Common.Documents
{
	public abstract class TypeDeviationMeasure : ProjectDocument
	{
		public string NamespaceName { get; set; }

		public string TypeName { get; set; }

		public double Sigma { get; set; }
	}
}