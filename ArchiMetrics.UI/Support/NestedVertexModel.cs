// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelNodeModel.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ModelNodeModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.UI.Support
{
	using System;
	using System.Collections.ObjectModel;

	[Serializable]
	public class ModelNodeModel
	{
		public string Name { get; set; }

		public string Type { get; set; }

		public Collection<ModelNodeModel> Children { get; set; }

		public int LinesOfCode { get; set; }

		public double MaintainabilityIndex { get; set; }

		public int CyclomaticComplexity { get; set; }

		public string Quality { get; set; }
	}
}