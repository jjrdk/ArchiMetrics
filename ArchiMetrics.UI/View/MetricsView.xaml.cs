using System.IO;
using System.Windows.Forms;
using System.Windows.Input;

namespace ArchiMetrics.UI.View
{
	using System;
	using System.Windows;
	using System.Windows.Controls;

	/// <summary>
	/// Interaction logic for MetricsView.xaml
	/// </summary>
	public partial class MetricsView : UserControl
	{
		public MetricsView()
		{
			InitializeComponent();
		}

		private async void OnPrintReport(object sender, RoutedEventArgs e)
		{
			var saveDialog = new SaveFileDialog();
			if (saveDialog.ShowDialog() == DialogResult.OK)
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
