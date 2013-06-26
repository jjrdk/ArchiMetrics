// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VertexRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the VertexRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMetrics.Common
{
	using System.Xml.Serialization;

	[XmlRoot(ElementName = "VertexRule")]
	public class VertexRule
	{
		[XmlAttribute]
		public string Pattern { get; set; }

		[XmlAttribute]
		public string Name { get; set; }
	}
}
