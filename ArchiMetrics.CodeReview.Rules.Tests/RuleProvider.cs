namespace ArchiMetrics.CodeReview.Rules.Tests
{
	using System.Collections;
	using System.Linq;

	public static class RuleProvider
	{
		public static IEnumerable Rules
		{
			get
			{
				return AllRules.GetRules()
					.Where(x => x.GetConstructors().Any(c => c.GetParameters().Length == 0));
			}
		}
	}
}