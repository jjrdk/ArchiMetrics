// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SettingsView.xaml.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Interaction logic for SettingsView.xaml.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.UI.View
{
	using System.Windows;
	using System.Windows.Controls;
	using System.Windows.Forms;
	using ArchiMetrics.UI.Support;
	using ArchiMetrics.UI.ViewModel;
	using UserControl = System.Windows.Controls.UserControl;

	/// <summary>
	/// Interaction logic for SettingsView.xaml.
	/// </summary>
	[DataContext(typeof(SettingsViewModel))]
	public partial class SettingsView : UserControl
	{
		public SettingsView()
		{
			InitializeComponent();
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
