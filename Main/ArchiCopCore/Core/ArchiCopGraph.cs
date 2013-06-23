using System.Collections.Generic;
using System.Linq;
using QuickGraph;
using QuickGraph.Algorithms;

namespace ArchiCop.Core
{
    public class ArchiCopGraph<T> : BidirectionalGraph<T, ArchiCopEdge<T>>
        where T : ArchiCopVertex
    {        
        public ArchiCopGraph()
        {
            
        }

        public ArchiCopGraph(IEnumerable<Edge<T>> edges)
        {
            foreach (T vertex in edges.Select(item => item.Source))
            {
                if (!ContainsVertex(vertex))
                {
                    AddVertex(vertex);
                }
            }

            foreach (T vertex in edges.Select(item => item.Target))
            {
                if (!ContainsVertex(vertex))
                {
                    AddVertex(vertex);
                }
            }

            foreach (var edge in edges)
            {
                AddEdge(new ArchiCopEdge<T>(edge.Source, edge.Target));
            }
        }
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