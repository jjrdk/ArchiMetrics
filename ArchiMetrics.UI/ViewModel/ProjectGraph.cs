// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectGraph.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ProjectGraph type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.UI.ViewModel
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
