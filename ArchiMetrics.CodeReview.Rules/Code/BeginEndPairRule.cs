namespace ArchiMetrics.CodeReview.Rules.Code
{
	internal class BeginEndPairRule : MethodNamePairRule
	{
		public override string Title
		{
			get
			{
				return "Begin/End Method Pair";
			}
		}

		public override string Suggestion
		{
			get
			{
				return "Methods names BeginSomething should have a matching EndSomething and vice versa.";
			}
		}

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
