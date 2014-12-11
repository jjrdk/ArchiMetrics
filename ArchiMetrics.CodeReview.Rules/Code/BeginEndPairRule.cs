// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BeginEndPairRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the BeginEndPairRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Rules.Code
{
	internal class BeginEndPairRule : MethodNamePairRule
	{
		public override string ID
		{
			get
			{
				return "AMC0036";
			}
		}

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
