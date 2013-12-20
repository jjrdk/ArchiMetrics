namespace SampleSL
{
	public class LocClass
	{
		public LocClass()
		{
		}

		public int Value { get; set; }

		public void SomeMethod() { const string x = "blah"; }

		public int GetValue() { return 1; }
		public int GetValue(int x) { return x + 1; }

		public double GetValue(double x)
		{
			if (x % 2 == 0.0)
			{
				return x;
			}
			return x + 1;
		}
	}
}