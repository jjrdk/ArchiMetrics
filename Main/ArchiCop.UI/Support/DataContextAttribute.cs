namespace ArchiMeter.UI.Support
{
	using System;

	public	class DataContextAttribute:Attribute
	{
		public DataContextAttribute(Type dataContextType)
		{
			this.DataContextType = dataContextType;
		}

		public Type DataContextType { get; set; }
	}
}
