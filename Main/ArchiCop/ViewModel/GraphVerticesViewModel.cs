using System.Collections.ObjectModel;
using ArchiCop.Core;

namespace ArchiCop.ViewModel
{
    public class GraphVerticesViewModel : GraphViewModel
    {
        public GraphVerticesViewModel(ArchiCopGraph<ArchiCopVertex> graph, string displayName, string tag) :
            base(graph, displayName, tag)
        {
            Vertices = new ObservableCollection<ArchiCopVertexViewModel>();
            foreach (ArchiCopVertex archiCopVertex in graph.Vertices)
            {
                Vertices.Add(new ArchiCopVertexViewModel(archiCopVertex));
            }
        }

        public ObservableCollection<ArchiCopVertexViewModel> Vertices { get; private set; }
    }
}