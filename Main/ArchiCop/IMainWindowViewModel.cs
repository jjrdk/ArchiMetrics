using System.Collections.ObjectModel;
using ArchiCop.ViewModel;

namespace ArchiCop
{
    public interface IMainWindowViewModel
    {
        /// <summary>
        ///     Returns a list of commands
        ///     that the UI can display and execute.
        /// </summary>
        ObservableCollection<CommandViewModel> ControlPanelCommands { get; }


        /// <summary>
        ///     Returns the collection of available workspaces to display.
        ///     A 'workspace' is a ViewModel that can request to be closed.
        /// </summary>
        ObservableCollection<WorkspaceViewModel> Workspaces { get; }
    }
}