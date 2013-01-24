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

    public class GraphViewModel : WorkspaceViewModel
    {
        public GraphViewModel(GraphEngine graphEngine, string displayName)
        {
            DisplayName = displayName;

            GraphToVisualize = graphEngine;        
        }

        public ArchiCopGraph GraphToVisualize { get; private set; }
        
    }
}