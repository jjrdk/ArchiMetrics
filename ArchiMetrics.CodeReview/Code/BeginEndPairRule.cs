namespace ArchiMetrics.CodeReview.Code
{
	internal class BeginEndPairRule : MethodNamePairRule
	{
		protected override string BeginToken
		{
			get { return "Begin"; }
		}

		protected override string PairToken
		{
			get { return "End"; }
		}
	}
}
