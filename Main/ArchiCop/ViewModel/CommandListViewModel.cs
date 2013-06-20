using System.Collections.ObjectModel;

namespace ArchiCop.ViewModel
{
    public class CommandListViewModel : ViewModelBase
    {
        public CommandListViewModel(string displayName)            
        {
            base.DisplayName = displayName;
            Commands = new ObservableCollection<CommandViewModel>();
        }

        public ObservableCollection<CommandViewModel> Commands { get; set; }

        public string Tag { get; set; }
    }
}