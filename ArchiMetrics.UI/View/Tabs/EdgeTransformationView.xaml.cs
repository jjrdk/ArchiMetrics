// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EdgeTransformationView.xaml.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Interaction logic for EdgeTransformationView.xaml.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.UI.View.Tabs
{
	using System.Windows;
	using System.Windows.Controls;
	using ArchiMetrics.UI.Support;
	using ArchiMetrics.UI.ViewModel;
	using Microsoft.Win32;

	/// <summary>
	/// Interaction logic for EdgeTransformationView.xaml.
	/// </summary>
	[DataContext(typeof(VertexViewModel))]
	public partial class EdgeTransformationView : UserControl
	{
		private const string FileFilter = "Transform Files (*.transform)|*.transform|All Files (*.*)|*.*";

		public EdgeTransformationView()
		{
			InitializeComponent();
		}

		private void OnClick(object sender, RoutedEventArgs e)
		{
			var fileDialog = new OpenFileDialog
			{
				Multiselect = false, 
				Filter = FileFilter
			};

			if (fileDialog.ShowDialog() == true)
			{
				PathBox.SetValue(TextBlock.TextProperty, fileDialog.FileName);
			}
		}

		private void OnSave(object sender, RoutedEventArgs e)
		{
			var fileDialog = new SaveFileDialog
			{
				AddExtension = true, 
				Filter = FileFilter
			};

			if (fileDialog.ShowDialog() == true)
			{
				var context = (VertexViewModel)DataContext;
				context.SaveTransforms(fileDialog.FileName);
			}
		}
	}
}
