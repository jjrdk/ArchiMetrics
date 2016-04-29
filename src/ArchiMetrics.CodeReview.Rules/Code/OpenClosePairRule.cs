// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OpenClosePairRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the OpenClosePairRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Rules.Code
{
	internal class OpenClosePairRule : MethodNamePairRule
	{
		public override string ID
		{
			get
			{
				return "AM0035";
			}
		}

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
