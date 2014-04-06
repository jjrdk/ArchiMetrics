namespace ArchiMetrics.CodeReview.Rules.Code
{
	internal class LeakingUnityContainerRule : LeakingTypeRule
	{
		protected override string TypeIdentifier
		{
			get
			{
				return "UnityContainer";
			}
		}
	}
}
