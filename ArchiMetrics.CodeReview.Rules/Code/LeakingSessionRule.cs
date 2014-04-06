namespace ArchiMetrics.CodeReview.Rules.Code
{
	internal class LeakingSessionRule : LeakingTypeRule
	{
		protected override string TypeIdentifier
		{
			get
			{
				return "ISession";
			}
		}
	}
}
