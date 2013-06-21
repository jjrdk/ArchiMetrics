using System.Windows.Input;
using MvvmFoundation.Wpf;

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

            ShowCommand=new RelayCommand(() =>
                {
                    App.Messenger.NotifyColleagues(App.CLEAR_WORKSPACES);
                    App.Messenger.NotifyColleagues(App.SET_WORKSPACES_DISPLAYTEXT, displayName);
                    showGraphCommand.Execute(null);
                    showEdgesCommand.Execute(null);
                });

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

        public ICommand ShowCommand { get; private set; }
        
        public string Tag { get; set; }

        public string ImageSource { get; private set; }
    }
}