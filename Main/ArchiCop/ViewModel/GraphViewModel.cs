using ArchiCop.Core;

namespace ArchiCop.ViewModel
{
    public class GraphViewModel : WorkspaceViewModel
    {
        private string _layoutAlgorithmType = "KK";

        public GraphViewModel(GraphEngine graphEngine, string displayName)
        {
            DisplayName = displayName;

            GraphToVisualize = graphEngine.Graph;
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
        public ArchiCopGraph GraphToVisualize { get; private set; }
    }
}