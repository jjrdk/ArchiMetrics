namespace ArchiMetrics.Common.Xaml
{
	public class XamlPropertyNode
	{
		public XamlPropertyNode(string propertyName, bool isDependency, object value)
		{
			Name = propertyName;
			Value = value;
			IsDependency = isDependency;
		}

		public string Name { get; private set; }

		public bool IsDependency { get; private set; }

		public object Value { get; private set; }
	}
}
