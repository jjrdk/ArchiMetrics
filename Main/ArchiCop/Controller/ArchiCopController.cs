using System.Collections.Generic;
using System.IO;
using ArchiCop.InfoData;
using ArchiCop.ViewModel;
using Microsoft.Practices.Unity;
using MvvmFoundation.Wpf;

namespace ArchiCop.Controller
{
    public class ArchiCopController : IController
    {
        private readonly IMainWindowViewModel _mainWindowViewModel;

        public ArchiCopController(IMainWindowViewModel mainWindowViewModel, ArchiCopSolutionViewModel archiCopSolutionViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;

            _mainWindowViewModel.ControlPanelCommands.Add(archiCopSolutionViewModel);

        }
        
    }
}