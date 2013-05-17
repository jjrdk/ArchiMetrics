namespace ArchiMeter.Reports.Metrics
{
	public class MetricResult
	{
		// Properties
		public string Name { get; set; }

		public object Value { get; set; }

		public string ValueText
		{
			get
			{
				if (this.Value != null)
				{
					return this.Value.ToString();
				}
				return null;
			}
		}
	}
}