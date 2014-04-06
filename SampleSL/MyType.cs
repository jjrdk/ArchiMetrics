namespace SampleSL
{
	using System;

	public class MyType
	{
		public string Value { get; set; }

		public void DoSomething()
		{
			try
			{
				var x = 1 + 2;
				var y = x + 2;
			}
			catch
			{
				throw new Exception();
			}
		}
	}
}
