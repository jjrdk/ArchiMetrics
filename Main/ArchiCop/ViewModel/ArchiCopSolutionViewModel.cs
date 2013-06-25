using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using ArchiCop.Controller;
using ArchiCop.Core;
using ArchiCop.Properties;
using Microsoft.Practices.Unity;
using MvvmFoundation.Wpf;

namespace ArchiCop.ViewModel
{
    public class ArchiCopSolutionViewModel : CommandViewModel
    {
        private readonly ObservableCollection<GraphCommandViewModel> _cachedCommands =
            new ObservableCollection<GraphCommandViewModel>();

        private readonly IMainWindowViewModel _mainWindowViewModel;

        [InjectionConstructor]
        public ArchiCopSolutionViewModel(IMainWindowViewModel mainWindowViewModel)
            : base(Resources.ArchiCopSolutionViewModel_DisplayName)
        {            
            _mainWindowViewModel = mainWindowViewModel;
            Commands = new ObservableCollection<GraphCommandViewModel>();

            Files = mainWindowViewModel.Configurations;

            ICollectionView filesView = CollectionViewSource.GetDefaultView(Files);
            filesView.CurrentChanged += FilesCurrentChanged;

            foreach (ConfigInfoViewModel configInfo in mainWindowViewModel.Configurations)
            {
                foreach (var dataSource in configInfo.DataSources)
                {
                    ICommand command1 = new RelayCommand<object>(param => ShowGraphView(dataSource.Graph));
                    ICommand command2 = new RelayCommand<object>(param => ShowGraphEdgesView(dataSource.Graph));
                    ICommand command3 = new RelayCommand<object>(param => ShowGraphVerticesView(dataSource.Graph));

                    _cachedCommands.Add(
                        new GraphCommandViewModel(dataSource.Graph.DisplayName, GraphCommandViewModelType.Datasource, command1,command2, command3)
                            {
                                Tag = configInfo.DisplayName
                            });

                    foreach (var graph in dataSource.Graphs)
                    {
                        ICommand command11 = new RelayCommand<object>(param => ShowGraphView(graph.Graph));
                        ICommand command12 = new RelayCommand<object>(param => ShowGraphEdgesView(graph.Graph));
                        ICommand command13 = new RelayCommand<object>(param => ShowGraphVerticesView(graph.Graph));

                        _cachedCommands.Add(
                            new GraphCommandViewModel(graph.DisplayName, GraphCommandViewModelType.Graph, command11, command12,command13)
                            {
                                Tag = configInfo.DisplayName
                            });
                    }
                }
                
            }
        }

        public ObservableCollection<GraphCommandViewModel> Commands { get; private set; }

        public ObservableCollection<ConfigInfoViewModel> Files { get; set; }

        private void FilesCurrentChanged(object sender, EventArgs e)
        {
            var tag = ((ICollectionView) sender).CurrentItem as ConfigInfoViewModel;

            Commands.Clear();


            foreach (
                GraphCommandViewModel commandViewModel in _cachedCommands.Where(item => item.Tag == tag.DisplayName))
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