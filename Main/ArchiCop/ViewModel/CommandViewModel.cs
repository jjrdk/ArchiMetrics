using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

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

    /// <summary>
    ///     Represents an actionable item displayed by a View.
    /// </summary>
    public class CommandViewModel : ViewModelBase
    {
        public CommandViewModel(string displayName)
        {
            base.DisplayName = displayName;
            
        }

        //public CommandViewModel(string displayName, ICommand command)
        //{
        //    if (command == null)
        //        throw new ArgumentNullException("command");

        //    base.DisplayName = displayName;
        //    Command = command;
        //}

        public ICommand Command { get; protected set; }

        public string Tag { get; set; }

    }
}