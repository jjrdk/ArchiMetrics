// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MetricsView.xaml.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Interaction logic for MetricsView.xaml.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.UI.View
{
	using System.IO;
	using System.Windows;
	using System.Windows.Controls;
	using System.Windows.Input;
	using ArchiMetrics.UI.Support;
	using ArchiMetrics.UI.ViewModel;
	using forms = System.Windows.Forms;

	/// <summary>
	/// Interaction logic for MetricsView.xaml.
	/// </summary>
	[DataContext(typeof(MetricsViewModel))]
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
