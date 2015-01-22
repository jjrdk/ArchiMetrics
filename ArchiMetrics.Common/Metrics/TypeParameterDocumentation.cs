namespace ArchiMetrics.Common.Metrics
{
	public class TypeParameterDocumentation
	{
		public TypeParameterDocumentation(string typeParameterName, string constraint, string description)
		{
			TypeParameterName = typeParameterName;
			Constraint = constraint;
			Description = description;
		}

		public string TypeParameterName { get; set; }

		public string Constraint { get; set; }

		public string Description { get; set; }
	}
}