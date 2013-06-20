using System.Collections.Generic;

namespace ArchiCop.Core
{
    public interface IEdgeEngine
    {
        IEnumerable<ArchiCopEdge<ArchiCopVertex>> ConvertEdges(IEnumerable<ArchiCopEdge<ArchiCopVertex>> edges, IEnumerable<VertexRegexRule> rules);
    }
}