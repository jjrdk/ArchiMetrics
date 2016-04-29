// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BranchResult.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the BranchResult type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Validation
{
    using Common.Structure;

    internal class BranchResult : ValidationResultBase
	{
		public BranchResult(bool passed, IModelNode vertex)
			: base(passed, vertex)
		{
		}

		public override string Value
		{
			get
			{
				return Passed ? "Pattern not recognized." : "Pattern recognized.";
			}
		}
	}
}