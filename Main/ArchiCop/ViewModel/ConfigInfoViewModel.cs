using System.Collections.ObjectModel;

namespace ArchiCop.ViewModel
{
    public class ConfigInfoViewModel : ViewModelBase
    {
        public ConfigInfoViewModel(string displayName)
        {
            DisplayName = displayName;
            DataSources = new ObservableCollection<DataSourceInfoViewModel>();
        }

        public ObservableCollection<DataSourceInfoViewModel> DataSources { get; private set; }
    }
}