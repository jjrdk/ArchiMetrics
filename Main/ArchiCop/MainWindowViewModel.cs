using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Data;
using ArchiCop.Core;
using ArchiCop.Data;
using ArchiCop.ViewModel;
using MvvmFoundation.Wpf;

namespace ArchiCop
{
    public class MainWindowViewModel : WorkspaceViewModel, IMainWindowViewModel
    {
        private ObservableCollection<CommandViewModel> _commands;
        private ObservableCollection<WorkspaceViewModel> _workspaces;

        public MainWindowViewModel(IInfoRepository repository)
        {
            base.DisplayName = "Strings.MainWindowViewModel_DisplayName";

            foreach (string excelSheetName in repository.GetGraphNames())
            {
                GraphInfo info = repository.GetGraphInfoData(excelSheetName);

                Commands.Add(
                    new CommandViewModel("Graph " + info.DisplayName,
                                         new RelayCommand<object>(param => ShowGraphView(info))));
            }

            foreach (string excelSheetName in repository.GetGraphNames())
            {
                GraphInfo info = repository.GetGraphInfoData(excelSheetName);

                Commands.Add(
                    new CommandViewModel("Edges " + info.DisplayName,
                                         new RelayCommand<object>(param => ShowGraphEdgesView(info))));
            }
        }

        /// <summary>
        ///     Returns a list of commands
        ///     that the UI can display and execute.
        /// </summary>
        public ObservableCollection<CommandViewModel> Commands
        {
            get
            {
                if (_commands == null)
                {
                    _commands = new ObservableCollection<CommandViewModel>();
                }
                return _commands;
            }
        }


        /// <summary>
        ///     Returns the collection of available workspaces to display.
        ///     A 'workspace' is a ViewModel that can request to be closed.
        /// </summary>
        public ObservableCollection<WorkspaceViewModel> Workspaces
        {
            get
            {
                if (_workspaces == null)
                {
                    _workspaces = new ObservableCollection<WorkspaceViewModel>();
                    _workspaces.CollectionChanged += OnWorkspacesChanged;
                }
                return _workspaces;
            }
        }

        private void ShowGraphView(GraphInfo info)
        {
            var workspace =
                Workspaces.Where(vm => vm is GraphViewModel).
                           FirstOrDefault(vm => vm.DisplayName == "Graph " + info.DisplayName) as GraphViewModel;

            if (workspace == null)
            {
                workspace = new GraphViewModel(new GraphEngine(info), info.DisplayName);
                Workspaces.Add(workspace);
            }

            SetActiveWorkspace(workspace);
        }

        private void ShowGraphEdgesView(GraphInfo info)
        {
            var workspace =
                Workspaces.Where(vm => vm is GraphEdgesViewModel).
                           FirstOrDefault(vm => vm.DisplayName == "Edges" + info.DisplayName) as GraphEdgesViewModel;

            if (workspace == null)
            {
                workspace = new GraphEdgesViewModel(new GraphEngine(info), info.DisplayName);
                Workspaces.Add(workspace);
            }

            SetActiveWorkspace(workspace);
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

        private void SetActiveWorkspace(WorkspaceViewModel workspace)
        {
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(Workspaces);
            if (collectionView != null)
                collectionView.MoveCurrentTo(workspace);
        }
    }
}