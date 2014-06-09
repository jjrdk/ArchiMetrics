// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KpiResult.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the KpiResult type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Validation
{
	using ArchiMetrics.Common.Structure;

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