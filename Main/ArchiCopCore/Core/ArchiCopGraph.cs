using System.Collections.Generic;
using QuickGraph;
using QuickGraph.Algorithms;

namespace ArchiCop.Core
{
    public class ArchiCopGraph : BidirectionalGraph<ArchiCopVertex, ArchiCopEdge>
    {
        private readonly IEnumerable<ArchiCopVertex> _oddVertices;
        private readonly IEnumerable<ArchiCopVertex> _roots;
        private readonly IEnumerable<ArchiCopVertex> _sinks;
        private readonly IDictionary<ArchiCopVertex, int> _stronglyConnectedComponents;
        private readonly IEnumerable<ArchiCopVertex> _topologicalSort;

        public ArchiCopGraph()
        {
            this.StronglyConnectedComponents(out _stronglyConnectedComponents);
            _topologicalSort = this.TopologicalSort();
            _roots = this.Roots();
            _sinks = this.Sinks();
            _oddVertices = this.OddVertices();
        }

        public string DisplayName { get; set; }

        public IDictionary<ArchiCopVertex, int> StronglyConnectedComponents
        {
            get { return _stronglyConnectedComponents; }
        }

        public IEnumerable<ArchiCopVertex> TopologicalSort
        {
            get { return _topologicalSort; }
        }

        public IEnumerable<ArchiCopVertex> Roots
        {
            get { return _roots; }
        }

        public IEnumerable<ArchiCopVertex> Sinks
        {
            get { return _sinks; }
        }

        public IEnumerable<ArchiCopVertex> OddVertices
        {
            get { return _oddVertices; }
        }
        
    }
}