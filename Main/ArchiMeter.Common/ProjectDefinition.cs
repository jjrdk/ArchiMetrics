// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectDefinition.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ProjectDefinition type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMeter.Common
{
	using System.Xml.Serialization;

	public class ProjectDefinition
	{
		[XmlAttribute]
		public string Source { get; set; }

		[XmlAttribute]
		public bool IsTest { get; set; }
	}

	public class TfsDefinition
	{
		[XmlAttribute]
		public string Definition { get; set; }
	}
}