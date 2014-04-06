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
	/// Interaction logic for CodeReviewDataGridView.xaml.
	/// </summary>
	[DataContext(typeof(CodeReviewViewModel))]
	public partial class CodeReviewDataGridView : UserControl
	{
		public CodeReviewDataGridView()
		{
			InitializeComponent();
		}

		private async void OnPrintReport(object sender, RoutedEventArgs e)
		{
			var saveDialog = new SaveFileDialog();
			if (saveDialog.ShowDialog() == true)
			{
				CodeReviewGrid.SelectAllCells();
				ApplicationCommands.Copy.Execute(null, CodeReviewGrid);
				CodeReviewGrid.UnselectAllCells();
				var data = (string)Clipboard.GetData(DataFormats.Html);
				Clipboard.Clear();
				var writer = new StreamWriter(saveDialog.FileName);
				await writer.WriteAsync(data);
				writer.Close();
			}
		}
	}
}
