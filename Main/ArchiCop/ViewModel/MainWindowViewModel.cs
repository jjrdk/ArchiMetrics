using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Data;
using ArchiCop.Core;
using ArchiCop.Data;
using ArchiCop.Properties;
using MvvmFoundation.Wpf;

namespace ArchiCop.ViewModel
{
    public class MainWindowViewModel : WorkspaceViewModel, IMainWindowViewModel
    {
        private readonly ICollectionView _metadataFilesView;
        private ObservableCollection<CommandViewModel> _controlPanelCommands;
        private IInfoRepository _repository;
        private ObservableCollection<WorkspaceViewModel> _workspaces;

        public MainWindowViewModel()
        {
            base.DisplayName = Resources.MainWindowViewModel_DisplayName;

            MetadataFiles = new ObservableCollection<string>(Directory.GetFiles(".", "*.xls"));
            _metadataFilesView = CollectionViewSource.GetDefaultView(MetadataFiles);
            _metadataFilesView.CurrentChanged += MetadataFilesCurrentChanged;
        }


        public ObservableCollection<string> MetadataFiles { get; private set; }

        #region IMainWindowViewModel Members

        /// <summary>
        ///     Returns a list of commands
        ///     that the UI can display and execute.
        /// </summary>
        public ObservableCollection<CommandViewModel> ControlPanelCommands
        {
            get
            {
                if (_controlPanelCommands == null)
                {
                    _controlPanelCommands = new ObservableCollection<CommandViewModel>();
                }
                return _controlPanelCommands;
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

        #endregion

        private void MetadataFilesCurrentChanged(object sender, EventArgs e)
        {
            _repository = new ExcelInfoRepository(_metadataFilesView.CurrentItem as string);

            ControlPanelCommands.Clear();

            foreach (ArchiCopGraph graph in new GraphService(_repository).Graphs)
            {
                ControlPanelCommands.Add(
                    new CommandViewModel("Graph " + graph.DisplayName,
                                         new RelayCommand<object>(param => ShowGraphView(graph))));

                ControlPanelCommands.Add(
                    new CommandViewModel("Edges " + graph.DisplayName,
                                         new RelayCommand<object>(param => ShowGraphEdgesView(graph))));
            }
        }

        private void ShowGraphView(ArchiCopGraph graph)
        {
            var workspace =
                Workspaces.Where(vm => vm is GraphViewModel).
                           FirstOrDefault(vm => vm.DisplayName == "Graph " + graph.DisplayName) as GraphViewModel;

            if (workspace == null)
            {
                workspace = new GraphViewModel(graph, "Graph " + graph.DisplayName);
                Workspaces.Add(workspace);
            }

            SetActiveWorkspace(workspace);
        }

        private void ShowGraphEdgesView(ArchiCopGraph graph)
        {
            var workspace =
                Workspaces.Where(vm => vm is GraphDetailsViewModel).
                           FirstOrDefault(vm => vm.DisplayName == "Edges" + graph.DisplayName) as GraphDetailsViewModel;

            if (workspace == null)
            {
                workspace = new GraphDetailsViewModel(graph, "Edges" + graph.DisplayName);
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