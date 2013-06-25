// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectReference.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ProjectReference type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMeter.Data.DataAccess
{
	using System.Collections.Generic;

	public class ProjectReference
	{
		public string ProjectPath { get; set; }

		public string Version { get; set; }

		public string Name { get; set; }

		public IEnumerable<KeyValuePair<string, string>> ProjectReferences { get; set; }

		public IEnumerable<string> AssemblyReferences { get; set; }
	}
}