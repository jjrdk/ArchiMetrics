using ArchiCop.Core;

namespace ArchiCop.ViewModel
{
    public class ArchiCopVertexViewModel : ViewModelBase
    {
        public ArchiCopVertexViewModel(ArchiCopVertex vertex)
        {
            Name = vertex.Name;
        }

        public string Name { get; private set; }
    }
}