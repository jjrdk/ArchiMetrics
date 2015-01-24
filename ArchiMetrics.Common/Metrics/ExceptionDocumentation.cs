namespace ArchiMetrics.Common.Metrics
{
	public class ExceptionDocumentation
	{
		public ExceptionDocumentation(string exceptionType, string description)
		{
			ExceptionType = exceptionType;
			Description = description;
		}

		public string ExceptionType { get; private set; }

		public string Description { get; private set; }
	}
}