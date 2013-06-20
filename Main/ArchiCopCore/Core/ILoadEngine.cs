using System.Collections.Generic;

namespace ArchiCop.Core
{
    public interface ILoadEngine
    {
        IEnumerable<ArchiCopEdge<ArchiCopVertex>> LoadEdges();
    }
}