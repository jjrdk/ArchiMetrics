namespace ArchiMetrics.Common.Metrics
{
	public class ParameterDocumentation
	{
		public string ParameterName { get; set; }

		public string ParameterType { get; set; }

		public string Description { get; set; }

		public ParameterDocumentation(string parameterName, string parameterType, string description)
		{
			ParameterName = parameterName;
			ParameterType = parameterType;
			Description = description;
		}
	}
}