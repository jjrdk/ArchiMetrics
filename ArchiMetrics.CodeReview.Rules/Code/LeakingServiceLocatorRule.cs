namespace ArchiMetrics.CodeReview.Rules.Code
{
	internal class LeakingServiceLocatorRule : LeakingTypeRule
	{
		protected override string TypeIdentifier
		{
			get
			{
				return "ServiceLocator";
			}
		}
	}
}
