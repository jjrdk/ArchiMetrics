// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NamespaceReference.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the NamespaceReference type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.UI.DataAccess
{
	using System.Collections.Generic;

	public class NamespaceReference
	{
		public string ProjectPath { get; set; }

		public string ProjectVersion { get; set; }

		public string Source { get; set; }

		public string Namespace { get; set; }

		public IEnumerable<string> References { get; set; }
	}
}
