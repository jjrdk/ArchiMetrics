// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LeakingDomainStorageRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the LeakingDomainStorageRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Rules.Code
{
	internal class LeakingDomainStorageRule : LeakingTypeRule
	{
		public override string ID
		{
			get
			{
				return "AM0019";
			}
		}

		protected override string TypeIdentifier
		{
			get
			{
				return "DomainStorage";
			}
		}
	}
}
