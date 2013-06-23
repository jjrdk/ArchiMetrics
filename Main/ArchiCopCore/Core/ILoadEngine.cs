using System.Collections.Generic;

namespace ArchiCop.Core
{
    public interface ILoadEngine
    {
        ArchiCopGraph<ArchiCopVertex> LoadGraph();
    }
}