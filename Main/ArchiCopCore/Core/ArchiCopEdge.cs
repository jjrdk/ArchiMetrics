using QuickGraph;

namespace ArchiCop.Core
{
    public class ArchiCopEdge : Edge<ArchiCopVertex>
    {
        public ArchiCopEdge(ArchiCopVertex source, ArchiCopVertex target)
            : base(source, target)
        {
        }
    }
}