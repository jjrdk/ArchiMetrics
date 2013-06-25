using System.Collections.ObjectModel;
using ArchiCop.Core;

namespace ArchiCop.ViewModel
{
    public enum DataSourceType
    {
        Data,
        VisualStudio
    }

    public class DataSourceInfoViewModel : ViewModelBase
    {
        public DataSourceInfoViewModel(string displayName, DataSourceType dataSourceType)
        {
            DisplayName = displayName;
            DataSourceType = dataSourceType;

            Graphs = new ObservableCollection<GraphInfoViewModel>();
        }

        public DataSourceType DataSourceType { get; private set; }

        public ArchiCopGraph<ArchiCopVertex> Graph { get; set; }

        public ObservableCollection<GraphInfoViewModel> Graphs { get; private set; }
    }
}