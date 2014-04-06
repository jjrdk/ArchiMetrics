namespace ArchiMetrics.UI.View.Tabs
{
	using System.IO;
	using System.Windows;
	using System.Windows.Controls;
	using System.Windows.Input;
	using ArchiMetrics.UI.Support;
	using ArchiMetrics.UI.ViewModel;
	using Microsoft.Win32;

	/// <summary>
	/// Interaction logic for MetricsMemberDataGridView.xaml.
	/// </summary>
	[DataContext(typeof(ProjectMetricsDataGridViewModel))]
	public partial class MetricsProjectDataGridView : UserControl
	{
		public MetricsProjectDataGridView()
		{
			InitializeComponent();
		}

		private async void OnPrintReport(object sender, RoutedEventArgs e)
		{
			var saveDialog = new SaveFileDialog();
			if (saveDialog.ShowDialog() == true)
			{
				MetricsGrid.SelectAllCells();
				ApplicationCommands.Copy.Execute(null, MetricsGrid);
				MetricsGrid.UnselectAllCells();
				var data = (string)Clipboard.GetData(DataFormats.Html);
				Clipboard.Clear();
				var writer = new StreamWriter(saveDialog.FileName);
				await writer.WriteAsync(data);
				writer.Close();
			}
		}
	}
}
