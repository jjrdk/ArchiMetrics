namespace ArchiMetrics.UI.View.Tabs
{
	using System.Windows;
	using System.Windows.Controls;
	using ArchiMetrics.UI.Support;
	using ArchiMetrics.UI.ViewModel;
	using Microsoft.Win32;

	/// <summary>
	/// Interaction logic for SettingsBasic.xaml.
	/// </summary>
	[DataContext(typeof(SettingsViewModel))]
	public partial class SettingsBasic : UserControl
	{
		public SettingsBasic()
		{
			InitializeComponent();
		}

		private void OnClick(object sender, RoutedEventArgs e)
		{
			var fileDialog = new OpenFileDialog
			{
				Multiselect = false,
				Filter = "Solution Files (*.sln)|*.sln|All Files (*.*)|*.*"
			};

			if (fileDialog.ShowDialog() == true)
			{
				PathBox.SetValue(TextBlock.TextProperty, fileDialog.FileName);
			}
		}
	}
}
