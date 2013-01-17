using System;
using System.Windows.Input;
using MvvmFoundation.Wpf;

namespace ArchiCop.ViewModel
{
    /// <summary>
    ///     This ViewModelBase subclass requests to be removed
    ///     from the UI when its CloseCommand executes.
    ///     This class is abstract.
    /// </summary>
    public abstract class WorkspaceViewModel : ViewModelBase
    {
        #region Fields

        private RelayCommand<object> _closeCommand;

        #endregion // Fields

        #region Constructor

        #endregion // Constructor

        #region CloseCommand

        /// <summary>
        ///     Returns the command that, when invoked, attempts
        ///     to remove this workspace from the user interface.
        /// </summary>
        public ICommand CloseCommand
        {
            get
            {
                if (_closeCommand == null)
                    _closeCommand = new RelayCommand<object>(param => OnRequestClose());

                return _closeCommand;
            }
        }

        #endregion // CloseCommand

        #region RequestClose [event]

        /// <summary>
        ///     Raised when this workspace should be removed from the UI.
        /// </summary>
        public event EventHandler RequestClose;

        private void OnRequestClose()
        {
            EventHandler handler = RequestClose;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        #endregion // RequestClose [event]
    }
}