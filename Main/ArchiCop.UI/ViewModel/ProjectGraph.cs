// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectGraph.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ProjectGraph type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiCop.UI.ViewModel
{
	using QuickGraph;

	internal class ProjectGraph : BidirectionalGraph<Vertex, ProjectEdge>
	{
		public ProjectGraph()
			: base(false)
		{
		}
	}
}