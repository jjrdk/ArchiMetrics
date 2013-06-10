namespace ArchiMeter.UI.Support
{
	using System;

	[AttributeUsage(AttributeTargets.Class)]
	public sealed class DataContextAttribute : Attribute
	{
		public DataContextAttribute(Type dataContextType)
		{
			this.DataContextType = dataContextType;
		}

		public Type DataContextType { get; set; }
	}
}
