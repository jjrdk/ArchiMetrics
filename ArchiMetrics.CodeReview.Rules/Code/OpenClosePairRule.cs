namespace ArchiMetrics.CodeReview.Rules.Code
{
	internal class OpenClosePairRule : MethodNamePairRule
	{
		public override string Title
		{
			get
			{
				return "Open/Close Method Pair";
			}
		}

		public override string Suggestion
		{
			get
			{
				return "Methods names OpenSomething should have a matching CloseSomething and vice versa.";
			}
		}

		protected override string BeginToken
		{
			get { return "Open"; }
		}

		protected override string PairToken
		{
			get { return "Close"; }
		}
	}
}
