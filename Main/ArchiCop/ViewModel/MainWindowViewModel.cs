using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using ArchiCop.Properties;

namespace ArchiCop.ViewModel
{
    public class MainWindowViewModel : WorkspaceViewModel, IMainWindowViewModel
    {
        private string _workspaceDisplayText;

        public MainWindowViewModel()
        {
            base.DisplayName = Resources.MainWindowViewModel_DisplayName;

            ControlPanelCommands = new ObservableCollection<CommandViewModel>();

            Workspaces = new ObservableCollection<WorkspaceViewModel>();
            Workspaces.CollectionChanged += OnWorkspacesChanged;

            App.Messenger.Register(App.CLEAR_WORKSPACES, CloseWorkspaces);
            App.Messenger.Register<string>(App.SET_WORKSPACES_DISPLAYTEXT, item => WorkspaceDisplayText = item);
        }

        public string WorkspaceDisplayText
        {
            get { return _workspaceDisplayText; }
            private set
            {
                _workspaceDisplayText = value;
                RaisePropertyChanged("WorkspaceDisplayText");
            }
        }

        #region IMainWindowViewModel Members

        /// <summary>
        ///     Returns a list of commands
        ///     that the UI can display and execute.
        /// </summary>
        public ObservableCollection<CommandViewModel> ControlPanelCommands { get; private set; }

        /// <summary>
        ///     Returns the collection of available workspaces to display.
        ///     A 'workspace' is a ViewModel that can request to be closed.
        /// </summary>
        public ObservableCollection<WorkspaceViewModel> Workspaces { get; private set; }

        #endregion

        public void SetActiveWorkspace(WorkspaceViewModel workspace)
        {
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(Workspaces);
            if (collectionView != null)
            {
                collectionView.MoveCurrentTo(workspace);
            }
        }

        private void CloseWorkspaces()
        {
            foreach (WorkspaceViewModel workspaceViewModel in Workspaces.ToList())
            {
                workspaceViewModel.CloseCommand.Execute(null);
            }
        }

        private void OnWorkspacesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count != 0)
                foreach (WorkspaceViewModel workspace in e.NewItems)
                    workspace.RequestClose += OnWorkspaceRequestClose;

            if (e.OldItems != null && e.OldItems.Count != 0)
                foreach (WorkspaceViewModel workspace in e.OldItems)
                    workspace.RequestClose -= OnWorkspaceRequestClose;
        }

        private void OnWorkspaceRequestClose(object sender, EventArgs e)
        {
            var workspace = sender as WorkspaceViewModel;
            workspace.Dispose();
            Workspaces.Remove(workspace);
        }
    }
}