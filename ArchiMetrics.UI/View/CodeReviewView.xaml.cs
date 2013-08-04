// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CodeReviewView.xaml.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Interaction logic for CodeReviewView.xaml.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.UI.View
{
	using System.IO;
	using System.Windows;
	using System.Windows.Forms;
	using System.Windows.Input;
	using ArchiMetrics.UI.Support;
	using ArchiMetrics.UI.ViewModel;
	using Clipboard = System.Windows.Clipboard;
	using DataFormats = System.Windows.DataFormats;
	using UserControl = System.Windows.Controls.UserControl;

	/// <summary>
	/// Interaction logic for CodeReviewView.xaml.
	/// </summary>
	[DataContext(typeof(CodeReviewViewModel))]
	public partial class CodeReviewView : UserControl
	{
		public CodeReviewView()
		{
			InitializeComponent();
		}

		private async void OnPrintReport(object sender, RoutedEventArgs e)
		{
			var saveDialog = new SaveFileDialog();
			if (saveDialog.ShowDialog() == DialogResult.OK)
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
