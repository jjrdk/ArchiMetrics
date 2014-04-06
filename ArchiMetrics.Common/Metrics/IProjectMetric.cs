// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProjectMetric.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the IProjectMetric type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Common.Metrics
{
	using System.Collections.Generic;

	public interface IProjectMetric : ICodeMetric
	{
		IEnumerable<ITypeCoupling> ClassCouplings { get; }

		IEnumerable<INamespaceMetric> NamespaceMetrics { get; }

		IEnumerable<string> ReferencedProjects { get; }

		double RelationalCohesion { get; set; }
	}
}