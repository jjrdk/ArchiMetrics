// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EdgesView.xaml.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Interaction logic for DependencyItemsView.xaml.
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
	/// Interaction logic for DependencyItemsView.xaml.
	/// </summary>
	[DataContext(typeof(EdgesViewModel))]
	public partial class EdgesView : UserControl
	{
		public EdgesView()
		{
			InitializeComponent();
		}

		private void OnClick(object sender, RoutedEventArgs e)
		{
			using (var fileDialog = new OpenFileDialog())
			{
				fileDialog.Multiselect = false;
				fileDialog.Filter = "XML Files (*.xml)|*.xml|All Files (*.*)|*.*";
				if (fileDialog.ShowDialog() == DialogResult.OK)
				{
					PathBox.SetValue(TextBlock.TextProperty, fileDialog.FileName);
				}
			}
		}

		private void OnSave(object sender, RoutedEventArgs e)
		{
			using (var fileDialog = new SaveFileDialog())
			{
				fileDialog.Filter = "XML Files (*.xml)|*.xml|All Files (*.*)|*.*";
				if (fileDialog.ShowDialog() == DialogResult.OK)
				{
					var context = (EdgesViewModel)DataContext;
					context.SaveTransforms(fileDialog.FileName);
				}
			}
		}
	}
}
