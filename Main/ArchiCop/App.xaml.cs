using System;
using System.Globalization;
using System.Windows;
using System.Windows.Markup;
using ArchiCop.Controller;
using ArchiCop.View;
using ArchiCop.ViewModel;
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

            // Allow all controls in the window to 
            // bind to the ViewModel by setting the 
            // DataContext, which propagates down 
            // the element tree.
            window.DataContext = viewModel;

            new ArchiCopController(viewModel);

            window.Show();
        }
    }
}