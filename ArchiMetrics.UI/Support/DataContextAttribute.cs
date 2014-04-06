namespace ArchiMetrics.UI.Support
{
	using System;

	[AttributeUsage(AttributeTargets.Class)]
	public sealed class DataContextAttribute : Attribute
	{
		public DataContextAttribute(Type dataContextType)
		{
			DataContextType = dataContextType;
		}

		public Type DataContextType { get; private set; }
	}
}
