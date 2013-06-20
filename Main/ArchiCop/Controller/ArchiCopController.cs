using System;
using System.Collections.ObjectModel;
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
        private readonly ObservableCollection<CommandViewModel> _controlPanelCommands = new ObservableCollection<CommandViewModel>();
        readonly ObservableCollection<string> _metadataFiles = new ObservableCollection<string>();
        
        public ArchiCopController(IMainWindowViewModel mainWindowViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;

            foreach (string file in Directory.GetFiles(".", "*.xls"))
            {
                _metadataFiles.Add(file);
            }
            
            _mainWindowViewModel.ControlPanelCommands.Add(new MetadataFilesViewModel(_metadataFiles));

            ICollectionView metadataFilesView = CollectionViewSource.GetDefaultView(_metadataFiles);
            metadataFilesView.CurrentChanged += MetadataFilesCurrentChanged;

            foreach (string metadataFile in _metadataFiles)
            {
                IInfoRepository repository = new ExcelInfoRepository(metadataFile);

                foreach (ArchiCopGraph graph in new GraphService(repository).Graphs)
                {
                    _controlPanelCommands.Add(
                        new CommandViewModel("Graph " + graph.DisplayName,
                                             new RelayCommand<object>(param => ShowGraphView(graph)))
                            {
                                Tag = metadataFile
                            });

                    _controlPanelCommands.Add(
                        new CommandViewModel("Edges " + graph.DisplayName,
                                             new RelayCommand<object>(param => ShowGraphEdgesView(graph)))
                            {
                                Tag = metadataFile
                            });
                }
            }            
        }

        private void MetadataFilesCurrentChanged(object sender, EventArgs e)
        {
            string tag = ((ICollectionView)sender).CurrentItem as string;
            
            _mainWindowViewModel.ControlPanelCommands.Clear();
            _mainWindowViewModel.ControlPanelCommands.Add(new MetadataFilesViewModel(_metadataFiles));

            foreach (
                CommandViewModel commandViewModel in _controlPanelCommands.Where(item => item.Tag == tag))
            {
                _mainWindowViewModel.ControlPanelCommands.Add(commandViewModel);
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