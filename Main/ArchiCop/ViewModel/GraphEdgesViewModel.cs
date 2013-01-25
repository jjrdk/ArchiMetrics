using System.Collections.Generic;
using ArchiCop.Core;

namespace ArchiCop.ViewModel
{
    public class GraphEdgesViewModel : GraphViewModel
    {
        public GraphEdgesViewModel(GraphEngine graphEngine, string displayName):
            base(graphEngine, displayName)
        {
            EdgesToVisualize = graphEngine.Edges;
        }

        public IEnumerable<ArchiCopEdge> EdgesToVisualize { get; private set; }

    }
}