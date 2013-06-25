using ArchiCop.Core;

namespace ArchiCop.ViewModel
{
    public class GraphInfoViewModel : ViewModelBase
    {
        public GraphInfoViewModel(string displayName)
        {
            DisplayName = displayName;
        }
        public ArchiCopGraph<ArchiCopVertex> Graph { get; set; }
    }
}