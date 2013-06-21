using System.Collections.Generic;
using QuickGraph;
using QuickGraph.Algorithms;

namespace ArchiCop.Core
{
    public class ArchiCopGraph<T> : BidirectionalGraph<T, ArchiCopEdge<T>>
        where T : ArchiCopVertex
    {
        private readonly IEnumerable<T> _oddVertices;
        private readonly IEnumerable<T> _roots;
        private readonly IEnumerable<T> _sinks;
        private readonly IDictionary<T, int> _stronglyConnectedComponents;
        private readonly IEnumerable<T> _topologicalSort;

        public ArchiCopGraph()
        {
            this.StronglyConnectedComponents(out _stronglyConnectedComponents);
            _topologicalSort = this.TopologicalSort();
            _roots = this.Roots();
            _sinks = this.Sinks();
            _oddVertices = this.OddVertices();
        }

        public string DisplayName { get; set; }

        public IDictionary<T, int> StronglyConnectedComponents
        {
            get { return _stronglyConnectedComponents; }
        }

        public IEnumerable<T> TopologicalSort
        {
            get { return _topologicalSort; }
        }

        public IEnumerable<T> Roots
        {
            get { return _roots; }
        }

        public IEnumerable<T> Sinks
        {
            get { return _sinks; }
        }

        public IEnumerable<T> OddVertices
        {
            get { return _oddVertices; }
        }
    }
}