namespace ArchiMetrics.UI.View
{
	using System.IO;
	using forms = System.Windows.Forms;
	using System.Windows.Input;
	using System.Windows;
	using System.Windows.Controls;

	/// <summary>
	/// Interaction logic for MetricsView.xaml.
	/// </summary>
	public partial class MetricsView : UserControl
	{
		public MetricsView()
		{
			InitializeComponent();
		}

		private async void OnPrintReport(object sender, RoutedEventArgs e)
		{
			var saveDialog = new forms.SaveFileDialog();
			if (saveDialog.ShowDialog() == forms.DialogResult.OK)
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
