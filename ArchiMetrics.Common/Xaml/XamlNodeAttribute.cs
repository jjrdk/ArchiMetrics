namespace ArchiMetrics.Common.Xaml
{
	public class XamlNodeAttribute
	{
		public XamlNodeAttribute(string variableName, string value, string extraCode)
		{
			VariableName = variableName;
			Value = value;
			ExtraCode = extraCode;
		}

		public string VariableName { get; private set; }

		public string Value { get; private set; }

		public string ExtraCode { get; private set; }
	}
}
