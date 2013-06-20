using System.Windows.Input;

namespace ArchiCop.ViewModel
{
    public class GraphCommandViewModel : ViewModelBase
    {
        public GraphCommandViewModel(string displayName, ICommand showGraphCommand, ICommand showEdgesCommand)
        {
            base.DisplayName = displayName;

            ShowGraphCommand = showGraphCommand;
            ShowEdgesCommand = showEdgesCommand;
        }

        public ICommand ShowGraphCommand { get; private set; }
        public ICommand ShowEdgesCommand { get; private set; }
        public string Tag { get; set; }
    }
}