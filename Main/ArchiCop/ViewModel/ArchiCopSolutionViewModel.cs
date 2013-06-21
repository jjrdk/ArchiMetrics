using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using ArchiCop.Core;
using ArchiCop.Data;
using ArchiCop.Properties;
using MvvmFoundation.Wpf;

namespace ArchiCop.ViewModel
{
    public class ArchiCopSolutionViewModel : CommandViewModel
    {
        private readonly ObservableCollection<GraphCommandViewModel> _cachedCommands =
            new ObservableCollection<GraphCommandViewModel>();

        private readonly IMainWindowViewModel _mainWindowViewModel;

        public ArchiCopSolutionViewModel(IMainWindowViewModel mainWindowViewModel)
            : base(Resources.ArchiCopSolutionViewModel_DisplayName)
        {
            Files = new ObservableCollection<string>();

            foreach (string file in Directory.GetFiles(".", "*.xls"))
            {
                Files.Add(file);
            }
            _mainWindowViewModel = mainWindowViewModel;
            Commands = new ObservableCollection<GraphCommandViewModel>();

            ICollectionView filesView = CollectionViewSource.GetDefaultView(Files);
            filesView.CurrentChanged += FilesCurrentChanged;

            foreach (string file in Files)
            {
                IInfoRepository repository = new ExcelInfoRepository(file);
                var graphService = new GraphService(repository);

                foreach (var graph in graphService.DataSources)
                {
                    ICommand command1 = new RelayCommand<object>(param => ShowGraphView(graph));
                    ICommand command2 = new RelayCommand<object>(param => ShowGraphEdgesView(graph));
                    ICommand command3 = new RelayCommand<object>(param => ShowGraphVerticesView(graph));

                    _cachedCommands.Add(
                        new GraphCommandViewModel(graph.DisplayName, GraphCommandViewModelType.Datasource, command1,
                                                  command2, command3)
                            {
                                Tag = file
                            });
                }

                foreach (var graph in graphService.Graphs)
                {
                    ICommand command1 = new RelayCommand<object>(param => ShowGraphView(graph));
                    ICommand command2 = new RelayCommand<object>(param => ShowGraphEdgesView(graph));
                    ICommand command3 = new RelayCommand<object>(param => ShowGraphVerticesView(graph));

                    _cachedCommands.Add(
                        new GraphCommandViewModel(graph.DisplayName, GraphCommandViewModelType.Graph, command1, command2,
                                                  command3)
                            {
                                Tag = file
                            });
                }
            }
        }

        public ObservableCollection<GraphCommandViewModel> Commands { get; private set; }

        public ObservableCollection<string> Files { get; set; }

        private void FilesCurrentChanged(object sender, EventArgs e)
        {
            var tag = ((ICollectionView) sender).CurrentItem as string;

            Commands.Clear();


            foreach (
                GraphCommandViewModel commandViewModel in _cachedCommands.Where(item => item.Tag == tag))
            {
                Commands.Add(commandViewModel);
            }
        }


        private void ShowGraphView(ArchiCopGraph<ArchiCopVertex> graph)
        {
            var workspace =
                _mainWindowViewModel.Workspaces.Where(vm => vm is GraphViewModel).
                                     FirstOrDefault(vm => vm.Tag == "Graph" + graph.DisplayName) as
                GraphViewModel;

            if (workspace == null)
            {
                workspace = new GraphViewModel(graph, "Graph", "Graph" + graph.DisplayName);
                _mainWindowViewModel.Workspaces.Add(workspace);
            }

            _mainWindowViewModel.SetActiveWorkspace(workspace);
        }

        private void ShowGraphVerticesView(ArchiCopGraph<ArchiCopVertex> graph)
        {
            var workspace =
                _mainWindowViewModel.Workspaces.Where(vm => vm is GraphVerticesViewModel).
                                     FirstOrDefault(vm => vm.Tag == "Vertices" + graph.DisplayName) as
                GraphVerticesViewModel;

            if (workspace == null)
            {
                workspace = new GraphVerticesViewModel(graph, "Vertices", "Vertices" + graph.DisplayName);
                _mainWindowViewModel.Workspaces.Add(workspace);
            }

            _mainWindowViewModel.SetActiveWorkspace(workspace);
        }

        private void ShowGraphEdgesView(ArchiCopGraph<ArchiCopVertex> graph)
        {
            var workspace =
                _mainWindowViewModel.Workspaces.Where(vm => vm is GraphEdgesViewModel).
                                     FirstOrDefault(vm => vm.Tag == "Edges" + graph.DisplayName) as
                GraphEdgesViewModel;

            if (workspace == null)
            {
                workspace = new GraphEdgesViewModel(graph, "Edges", "Edges" + graph.DisplayName);
                _mainWindowViewModel.Workspaces.Add(workspace);
            }

            _mainWindowViewModel.SetActiveWorkspace(workspace);
        }
    }
}