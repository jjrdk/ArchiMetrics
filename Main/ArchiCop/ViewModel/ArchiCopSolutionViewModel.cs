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

                foreach (ArchiCopGraph graph in new GraphService(repository).Graphs)
                {
                    ICommand command1 = new RelayCommand<object>(param => ShowGraphView(graph));
                    ICommand command2 = new RelayCommand<object>(param => ShowGraphEdgesView(graph));
                    _cachedCommands.Add(
                        new GraphCommandViewModel(graph.DisplayName, command1, command2)
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