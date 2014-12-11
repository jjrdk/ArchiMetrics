// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LeakingServiceLocatorRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the LeakingServiceLocatorRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Rules.Code
{
	internal class LeakingServiceLocatorRule : LeakingTypeRule
	{
		public override string ID
		{
			get
			{
				return "AMC0020";
			}
		}

		protected override string TypeIdentifier
		{
			get
			{
				return "ServiceLocator";
			}
		}
	}
}
