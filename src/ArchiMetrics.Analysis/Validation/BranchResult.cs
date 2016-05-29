// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BranchResult.cs" company="Reimers.dk">
//   Copyright © Matthias Friedrich, Reimers.dk 2014
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
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