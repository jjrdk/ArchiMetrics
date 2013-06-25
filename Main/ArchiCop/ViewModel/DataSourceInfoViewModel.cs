using System.Collections.ObjectModel;
using ArchiCop.Core;

namespace ArchiCop.ViewModel
{
    public class DataSourceInfoViewModel : ViewModelBase
    {
        public DataSourceInfoViewModel(string displayName)
        {
            DisplayName = displayName;
            Graphs = new ObservableCollection<GraphInfoViewModel>();
        }

        public ArchiCopGraph<ArchiCopVertex> Graph { get; set; }

        public ObservableCollection<GraphInfoViewModel> Graphs { get; private set; }
    }
}