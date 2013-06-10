﻿namespace ArchiMeter.CodeReview.Rules
{
	internal class OpenClosePairRule : MethodNamePairRule
	{
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