using System.Collections.Generic;
using ArchiCop.Core;

namespace ArchiCop.ViewModel
{
    public class GraphDetailsViewModel : GraphViewModel
    {
        public GraphDetailsViewModel(GraphEngine graphEngine, string displayName) :
            base(graphEngine, displayName)
        {
            EdgesToVisualize = graphEngine.Graph.Edges;
            Sinks = graphEngine.Sinks;
            Roots = graphEngine.Roots;
            StronglyConnectedComponents = graphEngine.StronglyConnectedComponents;
            TopologicalSort = graphEngine.TopologicalSort;
            OddVertices = graphEngine.OddVertices;
        }

        public IEnumerable<ArchiCopEdge> EdgesToVisualize { get; private set; }
        public IEnumerable<ArchiCopVertex> Sinks { get; private set; }
        public IEnumerable<ArchiCopVertex> Roots { get; private set; }
        public IEnumerable<ArchiCopVertex> OddVertices { get; private set; }
        public IEnumerable<ArchiCopVertex> TopologicalSort { get; private set; }
        public IDictionary<ArchiCopVertex, int> StronglyConnectedComponents { get; private set; }
    }
}