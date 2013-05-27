// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the MainWindow type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiCop.UI
{
	using System.Windows;
	using System.Windows.Controls;
	using System.Windows.Forms;

	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			this.InitializeComponent();
		}

		private void OnClick(object sender, RoutedEventArgs e)
		{
			using (var directoryOpener = new FolderBrowserDialog())
			{
				if (directoryOpener.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					this.TxtFolder.SetValue(TextBlock.TextProperty, directoryOpener.SelectedPath);
				}
			}
		}
	}
}