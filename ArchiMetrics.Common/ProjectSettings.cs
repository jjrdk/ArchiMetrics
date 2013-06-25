// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectSettings.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ProjectSettings type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMetrics.Common
{
	using System;
	using System.Collections.ObjectModel;
	using System.Xml.Serialization;

	public class ProjectSettings
	{
		[XmlAttribute]
		public string Revision { get; set; }

		[XmlAttribute]
		public string Name { get; set; }

		[XmlAttribute]
		public string Tag { get; set; }

		[XmlAttribute]
		public DateTime Date { get; set; }

		[XmlElement("Root")]
		public Collection<ProjectDefinition> Roots { get; set; }

		[XmlElement("TfsDefinition")]
		public Collection<TfsDefinition> TfsDefinitions { get; set; }
	}
}
