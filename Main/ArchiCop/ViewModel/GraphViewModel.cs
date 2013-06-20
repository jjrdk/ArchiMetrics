using ArchiCop.Core;

namespace ArchiCop.ViewModel
{
    public class GraphViewModel : WorkspaceViewModel
    {
        private string _layoutAlgorithmType = "KK";

        public GraphViewModel(ArchiCopGraph<ArchiCopVertex> graph, string displayName)
        {
            DisplayName = displayName;

            GraphToVisualize = graph;
        }

        public string LayoutAlgorithmType
        {
            get { return _layoutAlgorithmType; }
            set
            {
                if (value != _layoutAlgorithmType)
                {
                    _layoutAlgorithmType = value;
                    RaisePropertyChanged("LayoutAlgorithmType");
                }
            }
        }

        public ArchiCopGraph<ArchiCopVertex> GraphToVisualize { get; private set; }
    }
}