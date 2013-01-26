using System.Collections.Generic;

namespace ArchiCop.Core
{
    public interface IEdgeEngine
    {
        IEnumerable<ArchiCopEdge> ConvertEdges(IEnumerable<ArchiCopEdge> edges, IEnumerable<VertexRegexRule> rules);
    }
}