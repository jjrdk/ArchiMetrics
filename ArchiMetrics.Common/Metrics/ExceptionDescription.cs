namespace ArchiMetrics.Common.Metrics
{
	public class ExceptionDescription
	{
		public ExceptionDescription(string exceptionType, string description)
		{
			ExceptionType = exceptionType;
			Description = description;
		}

		public string ExceptionType { get; private set; }

		public string Description { get; private set; }
	}
}