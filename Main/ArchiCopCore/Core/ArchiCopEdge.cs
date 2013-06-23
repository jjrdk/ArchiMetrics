using QuickGraph;

namespace ArchiCop.Core
{
    public class ArchiCopEdge<T> : Edge<T>
        where T : ArchiCopVertex
    {
        public ArchiCopEdge(T source, T target)
            : base(source, target)
        {
        }
    }
}