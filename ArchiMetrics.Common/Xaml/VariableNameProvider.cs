namespace ArchiMetrics.Common.Xaml
{
	using System.Collections.Generic;

	internal class VariableNameProvider
	{
		private static readonly Dictionary<string, int> UnnamedCache = new Dictionary<string, int>();

		public static string Get(string className)
		{
			if (UnnamedCache.ContainsKey(className) == false)
			{
				UnnamedCache.Add(className, 0);
			}

			var name = className.ToLower() + UnnamedCache[className];

			UnnamedCache[className]++;

			return name.Replace(".", "_");
		}
	}
}
