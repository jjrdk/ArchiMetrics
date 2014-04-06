namespace ArchiMetrics.CodeReview.Rules.Code
{
	internal class LeakingDomainStorageRule : LeakingTypeRule
	{
		protected override string TypeIdentifier
		{
			get
			{
				return "DomainStorage";
			}
		}
	}
}
