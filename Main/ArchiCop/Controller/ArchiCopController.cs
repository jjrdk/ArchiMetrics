using ArchiCop.ViewModel;

namespace ArchiCop.Controller
{
    public class ArchiCopController : IController
    {
        private readonly IMainWindowViewModel _mainWindowViewModel;

        public ArchiCopController(IMainWindowViewModel mainWindowViewModel,
                                  ArchiCopSolutionViewModel archiCopSolutionViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;
            _mainWindowViewModel.ControlPanelCommands.Add(archiCopSolutionViewModel);
        }
    }
}