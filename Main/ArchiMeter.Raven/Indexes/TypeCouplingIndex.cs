// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeCouplingIndex.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the TypeCouplingIndex type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMeter.Raven.Indexes
{
	using System.Linq;
	using Common.Documents;
	using Common.Metrics;
	using global::Raven.Client.Indexes;

	public class TypeCouplingIndex : AbstractIndexCreationTask<ProjectMetricsDocument, TypeCoupling>
	{
		public TypeCouplingIndex()
		{
			Map = docs => from doc in docs
						  from nm in doc.Metrics
						  let nmc = nm.ClassCouplings
						  from tm in nm.TypeMetrics
						  let tmc = tm.ClassCouplings
						  from mm in tm.MemberMetrics
						  let mmc = mm.ClassCouplings
						  let couplings = nmc.Concat(tmc).Concat(mmc)
						  from coupling in couplings
						  select new
									 {
										 coupling.Assembly,
										 coupling.Namespace,
										 coupling.ClassName
									 };
		}
	}
}