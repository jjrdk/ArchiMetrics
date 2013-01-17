using QuickGraph;

namespace ArchiCop
{
    public class ArchiCopEdge : Edge<ArchiCopVertex>
    {
        public ArchiCopEdge(ArchiCopVertex source, ArchiCopVertex target)
            : base(source, target)
        {
        }
    }
}