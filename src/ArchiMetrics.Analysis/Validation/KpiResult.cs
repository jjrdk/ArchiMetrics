// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KpiResult.cs" company="Reimers.dk">
//   Copyright © Matthias Friedrich, Reimers.dk 2014
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the KpiResult type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Validation
{
    using Common.Structure;

    internal class KpiResult : ValidationResultBase
	{
		public KpiResult(bool passed, IModelNode vertex)
			: base(passed, vertex)
		{
		}

		public override string Value
		{
			get
			{
				return "Found node outside quality range";
			}
		}
	}
}