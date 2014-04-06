namespace ArchiMetrics.UI.ViewModel
{
	using System.Collections.Generic;
	using ArchiMetrics.Common.Structure;
	using ArchiMetrics.UI.Support;
	using QuickGraph;

	internal class ModelGraph : BidirectionalGraph<IModelNode, ModelEdge>
	{
		public ModelGraph(IEnumerable<ModelEdge> edges)
			: base(false)
		{
			AddVerticesAndEdgeRange(edges);
		}
	}
}