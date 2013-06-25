// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectEdge.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ProjectEdge type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.UI.ViewModel
{
	using QuickGraph;

	internal class ProjectEdge : Edge<Vertex>
	{
		public ProjectEdge(Vertex source, Vertex target)
			: base(source, target)
		{
		}

		public bool IsCircular { get; set; }
	}
}
