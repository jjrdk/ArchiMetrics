using System.Windows.Input;

namespace ArchiCop.ViewModel
{
    public enum GraphCommandViewModelType
    {
        Graph,
        Datasource
    }

    public class GraphCommandViewModel : ViewModelBase
    {
        public GraphCommandViewModel(string displayName, GraphCommandViewModelType viewModelType,
                                     ICommand showGraphCommand, ICommand showEdgesCommand)
        {
            base.DisplayName = displayName;

            ShowGraphCommand = showGraphCommand;
            ShowEdgesCommand = showEdgesCommand;

            switch (viewModelType)
            {
                case GraphCommandViewModelType.Datasource:
                    ImageSource = "Avosoft-Warm-Toolbar-Database.ico";
                    break;
                case GraphCommandViewModelType.Graph:
                    ImageSource = "Icojam-Onebit-Diagram.ico";
                    break;
            }
        }

        public ICommand ShowGraphCommand { get; private set; }
        public ICommand ShowEdgesCommand { get; private set; }
        public string Tag { get; set; }

        public string ImageSource { get; private set; }
    }
}