using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Markup;
using ArchiCop.Controller;
using ArchiCop.Core;
using ArchiCop.InfoData;
using ArchiCop.View;
using ArchiCop.ViewModel;
using Microsoft.Practices.Unity;
using MvvmFoundation.Wpf;

namespace ArchiCop
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        internal const string CLEAR_WORKSPACES = "CLEAR_WORKSPACES";
        internal const string SET_WORKSPACES_DISPLAYTEXT = "SET_WORKSPACES_DISPLAYTEXT";
        private static readonly Messenger _messenger = new Messenger();

        static App()
        {
            // This code is used to test the app when using other cultures.
            //
            //System.Threading.Thread.CurrentThread.CurrentCulture =
            //    System.Threading.Thread.CurrentThread.CurrentUICulture =
            //        new System.Globalization.CultureInfo("it-IT");


            // Ensure the current culture passed into bindings is the OS culture.
            // By default, WPF uses en-US as the culture, regardless of the system settings.
            //
            FrameworkElement.LanguageProperty.OverrideMetadata(
                typeof (FrameworkElement),
                new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));
        }

        internal static Messenger Messenger
        {
            get { return _messenger; }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var window = new MainWindowView();

            var viewModel = new MainWindowViewModel();

            // When the ViewModel asks to be closed, 
            // close the window.
            EventHandler handler = null;
            handler = delegate
                {
                    viewModel.RequestClose -= handler;
                    window.Close();
                };
            viewModel.RequestClose += handler;
            
            IUnityContainer container = new UnityContainer();
            container.RegisterType<IInfoRepository, ExcelInfoRepository>(new ContainerControlledLifetimeManager());
            container.RegisterType<ArchiCopSolutionViewModel, ArchiCopSolutionViewModel>(
                new ContainerControlledLifetimeManager());
            container.RegisterInstance<IMainWindowViewModel>(viewModel);

            RegisterConfigInfoViewModels(container.Resolve<IInfoRepository>(), viewModel);

            container.Resolve<ArchiCopController>();
            
            // Allow all controls in the window to 
            // bind to the ViewModel by setting the 
            // DataContext, which propagates down 
            // the element tree.
            window.DataContext = viewModel;

            window.Show();
        }

        private void RegisterConfigInfoViewModels(IInfoRepository infoRepository, IMainWindowViewModel mainWindowViewModel)
        {
            var files = Directory.GetFiles(".", "*.xls");
            IEnumerable<ConfigInfo> configInfos = infoRepository.GetConfigInfos(files);

            foreach (ConfigInfo configInfo in configInfos)
            {
                var configInfoViewModel = new ConfigInfoViewModel(configInfo.DisplayName);

                foreach (DataSourceInfo dataSourceInfo in configInfo.DataSources)
                {
                    var dataSourceInfoViewModel = new DataSourceInfoViewModel(dataSourceInfo.DisplayName);

                    dataSourceInfoViewModel.Graph = ArchiCopGraphEngine.GetGraph(dataSourceInfo);

                    foreach (GraphInfo graphInfo in configInfo.Graphs)
                    {
                        if (graphInfo.DataSource.Name == dataSourceInfo.Name)
                        {
                            var graphInfoViewModel = new GraphInfoViewModel (graphInfo.DisplayName);

                            graphInfoViewModel.Graph = ArchiCopGraphEngine.GetGraph(graphInfo);

                            dataSourceInfoViewModel.Graphs.Add(graphInfoViewModel);
                        }
                    }

                    configInfoViewModel.DataSources.Add(dataSourceInfoViewModel);
                }

                mainWindowViewModel.Configurations.Add(configInfoViewModel);
            }
        }
    }
}