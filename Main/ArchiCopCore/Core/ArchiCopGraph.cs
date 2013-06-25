using System.Collections.Generic;
using QuickGraph;
using QuickGraph.Algorithms;

namespace ArchiCop.Core
{
    public class ArchiCopGraph<T> : BidirectionalGraph<T, ArchiCopEdge<T>>
        where T : ArchiCopVertex
    {
        public string DisplayName { get; set; }

        public IDictionary<T, int> StronglyConnectedComponents
        {
            get
            {
                IDictionary<T, int> stronglyConnectedComponents;
                this.StronglyConnectedComponents(out stronglyConnectedComponents);
                return stronglyConnectedComponents;
            }
        }

        public IEnumerable<T> TopologicalSort
        {
            get { return this.TopologicalSort(); }
        }

        public IEnumerable<T> Roots
        {
            get { return this.Roots(); }
        }

        public IEnumerable<T> Sinks
        {
            get { return this.Sinks(); }
        }

        public IEnumerable<T> OddVertices
        {
            get { return this.OddVertices(); }
        }
    }
}