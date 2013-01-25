using ArchiCop.Core;

namespace ArchiCop.ViewModel
{
    public class GraphViewModel : WorkspaceViewModel
    {
        public GraphViewModel(GraphEngine graphEngine, string displayName)
        {
            DisplayName = displayName;

            GraphToVisualize = graphEngine.Graph;        
        }

        public ArchiCopGraph GraphToVisualize { get; private set; }
        
    }
}