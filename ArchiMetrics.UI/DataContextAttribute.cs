namespace ArchiCop.UI
{
	using System;

	public	class DataContextAttribute:Attribute
	{
		public DataContextAttribute(Type dataContextType)
		{
			DataContextType = dataContextType;
		}

		public Type DataContextType { get; set; }
	}
}
