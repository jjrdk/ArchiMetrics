// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelGraph.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ModelGraph type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.UI.ViewModel
{
	using System.Collections.Generic;
	using ArchiMetrics.Common.Structure;
	using Common;
	using QuickGraph;
	using Support;

	internal class ModelGraph : BidirectionalGraph<IModelNode, ModelEdge>
	{
		public ModelGraph(IEnumerable<ModelEdge> edges)
			: base(false)
		{
			AddVerticesAndEdgeRange(edges);
		}
	}
}