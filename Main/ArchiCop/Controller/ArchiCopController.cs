using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Data;
using ArchiCop.Core;
using ArchiCop.Data;
using ArchiCop.ViewModel;
using MvvmFoundation.Wpf;

namespace ArchiCop.Controller
{
    public class ArchiCopController : IController
    {
        private readonly IMainWindowViewModel _mainWindowViewModel;

        private readonly ICollectionView _metadataFilesView;
        private IInfoRepository _repository;


        public ArchiCopController(IMainWindowViewModel mainWindowViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;

            foreach (string file in Directory.GetFiles(".", "*.xls"))
            {
                _mainWindowViewModel.MetadataFiles.Add(file);
            }

            _metadataFilesView = CollectionViewSource.GetDefaultView(_mainWindowViewModel.MetadataFiles);
            _metadataFilesView.CurrentChanged += MetadataFilesCurrentChanged;
        }

        private void MetadataFilesCurrentChanged(object sender, EventArgs e)
        {
            _repository = new ExcelInfoRepository(_metadataFilesView.CurrentItem as string);

            _mainWindowViewModel.ControlPanelCommands.Clear();

            foreach (ArchiCopGraph graph in new GraphService(_repository).Graphs)
            {
                _mainWindowViewModel.ControlPanelCommands.Add(
                    new CommandViewModel("Graph " + graph.DisplayName,
                                         new RelayCommand<object>(param => ShowGraphView(graph))));

                _mainWindowViewModel.ControlPanelCommands.Add(
                    new CommandViewModel("Edges " + graph.DisplayName,
                                         new RelayCommand<object>(param => ShowGraphEdgesView(graph))));
            }
        }


        private void ShowGraphView(ArchiCopGraph graph)
        {
            var workspace =
                _mainWindowViewModel.Workspaces.Where(vm => vm is GraphViewModel).
                                     FirstOrDefault(vm => vm.DisplayName == "Graph " + graph.DisplayName) as
                GraphViewModel;

            if (workspace == null)
            {
                workspace = new GraphViewModel(graph, "Graph " + graph.DisplayName);
                _mainWindowViewModel.Workspaces.Add(workspace);
            }

            _mainWindowViewModel.SetActiveWorkspace(workspace);
        }

        private void ShowGraphEdgesView(ArchiCopGraph graph)
        {
            var workspace =
                _mainWindowViewModel.Workspaces.Where(vm => vm is GraphDetailsViewModel).
                                     FirstOrDefault(vm => vm.DisplayName == "Edges" + graph.DisplayName) as
                GraphDetailsViewModel;

            if (workspace == null)
            {
                workspace = new GraphDetailsViewModel(graph, "Edges" + graph.DisplayName);
                _mainWindowViewModel.Workspaces.Add(workspace);
            }

            _mainWindowViewModel.SetActiveWorkspace(workspace);
        }
    }
}