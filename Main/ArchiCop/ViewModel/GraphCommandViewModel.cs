using System.Windows.Input;
using MvvmFoundation.Wpf;

namespace ArchiCop.ViewModel
{
    public enum GraphCommandViewModelType
    {
        Graph,
        Datasource,
        VisualStudioDatasource
    }

    public class GraphCommandViewModel : ViewModelBase
    {
        public GraphCommandViewModel(string displayName, GraphCommandViewModelType viewModelType,
                                     ICommand showGraphCommand, ICommand showEdgesCommand, ICommand showVerticesCommand)
        {
            base.DisplayName = displayName;

            ShowCommand = new RelayCommand(() =>
                {
                    App.Messenger.NotifyColleagues(App.CLEAR_WORKSPACES);
                    App.Messenger.NotifyColleagues(App.SET_WORKSPACES_DISPLAYTEXT, displayName);
                    showGraphCommand.Execute(null);
                    showEdgesCommand.Execute(null);
                    showVerticesCommand.Execute(null);
                });

            switch (viewModelType)
            {
                case GraphCommandViewModelType.Datasource:
                    ImageSource = "Avosoft-Warm-Toolbar-Database.ico";
                    break;
                case GraphCommandViewModelType.Graph:
                    ImageSource = "Icojam-Onebit-Diagram.ico";
                    break;
                    case GraphCommandViewModelType.VisualStudioDatasource:
                    ImageSource = "Dakirby309-Windows-8-Metro-Apps-Visual-Studio-alt-Metro.ico";
                    break;                    
            }
        }

        public ICommand ShowCommand { get; private set; }

        public string Tag { get; set; }

        public string ImageSource { get; private set; }
    }
}