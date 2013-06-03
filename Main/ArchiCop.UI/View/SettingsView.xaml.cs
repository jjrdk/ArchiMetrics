namespace ArchiMeter.UI.View
{
	using System.Windows;
	using System.Windows.Controls;
	using System.Windows.Forms;

	using ArchiMeter.UI.ViewModel;

	using UserControl = System.Windows.Controls.UserControl;

	/// <summary>
	/// Interaction logic for SettingsView.xaml
	/// </summary>
	[DataContext(typeof(SettingsViewModel))]
	public partial class SettingsView : UserControl
	{
		public SettingsView()
		{
			this.InitializeComponent();
		}

		private void OnClick(object sender, RoutedEventArgs e)
		{
			using (var directoryOpener = new FolderBrowserDialog())
			{
				if (directoryOpener.ShowDialog() == DialogResult.OK)
				{
					PathBox.SetValue(TextBlock.TextProperty, directoryOpener.SelectedPath);
				}
			}
		}
	}
}
