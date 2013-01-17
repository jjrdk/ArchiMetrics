using System.Collections.Generic;
using QuickGraph;

namespace ArchiCop
{
    public class ArchiCopGraph : BidirectionalGraph<ArchiCopVertex, ArchiCopEdge>
    {
        public ArchiCopGraph(IEnumerable<ArchiCopEdge> edges)
        {
            AddVerticesAndEdgeRange(edges);
        }
    }
}