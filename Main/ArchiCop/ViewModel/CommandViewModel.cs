using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ArchiCop.ViewModel
{
    /// <summary>
    ///     Represents an actionable item displayed by a View.
    /// </summary>
    public class CommandViewModel : ViewModelBase
    {
        public CommandViewModel(string displayName)
        {
            base.DisplayName = displayName;
            //TODO
            //Commands = new ObservableCollection<CommandViewModel>();
        }

        public CommandViewModel(string displayName, ICommand command)
        {
            if (command == null)
                throw new ArgumentNullException("command");

            base.DisplayName = displayName;
            Command = command;
        }

        public ICommand Command { get; protected set; }

        public string Tag { get; set; }

        //TODO
        //public ObservableCollection<CommandViewModel> Commands { get; protected set; }
    }
}