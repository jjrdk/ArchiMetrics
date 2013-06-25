// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelSettings.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ModelSettings type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMetrics.Common
{
	using System.Xml.Serialization;

	public class ModelSettings
	{
		[XmlAttribute("Name")]
		public string Name { get; set; }

		[XmlAttribute("Root")]
		public string Root { get; set; }

		[XmlAttribute("File")]
		public string File { get; set; }

		[XmlAttribute("DataFile")]
		public string Data { get; set; }
	}
}
