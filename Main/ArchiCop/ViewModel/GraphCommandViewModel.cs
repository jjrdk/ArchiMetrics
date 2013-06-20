using System.Windows.Input;

namespace ArchiCop.ViewModel
{
    public class GraphCommandViewModel : CommandListViewModel
    {
        public GraphCommandViewModel(string displayName, ICommand showGraphCommand, ICommand showEdgesCommand) : 
            base(displayName)
        {
            ShowGraphCommand = showGraphCommand;
            ShowEdgesCommand = showEdgesCommand;
        }

        public ICommand ShowGraphCommand { get; private set; }
        public ICommand ShowEdgesCommand { get; private set; }
    }
}