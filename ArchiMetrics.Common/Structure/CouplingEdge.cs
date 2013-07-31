// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CouplingEdge.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the CouplingEdge type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Common.Structure
{
	public class CouplingEdge
	{
		public string ProjectName { get; set; }

		public string ProjectVersion { get; set; }

		public string DependantNamespaceName { get; set; }

		public string DependantTypeName { get; set; }

		public string DependencyAssembly { get; set; }

		public string DependencyNamespaceName { get; set; }

		public string DependencyTypeName { get; set; }
	}
}
