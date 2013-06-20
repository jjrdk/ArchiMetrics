using System.Collections.ObjectModel;
using System.IO;
using ArchiCop.ViewModel;

namespace ArchiCop.Controller
{
    public class ArchiCopController : IController
    {
        public ArchiCopController(IMainWindowViewModel mainWindowViewModel)
        {
            mainWindowViewModel.ControlPanelCommands.Add(new ArchiCopSolutionViewModel(mainWindowViewModel));
        }
    }
}