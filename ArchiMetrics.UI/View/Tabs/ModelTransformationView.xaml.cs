namespace ArchiMetrics.UI.View.Tabs
{
	using System.Windows;
	using System.Windows.Controls;
	using ArchiMetrics.UI.Support;
	using ArchiMetrics.UI.ViewModel;
	using Microsoft.Win32;

	/// <summary>
	/// Interaction logic for ModelTransformationView.xaml.
	/// </summary>
	[DataContext(typeof(StructureRulesViewModel))]
	public partial class ModelTransformationView : UserControl
	{
		private const string FileFilter = "Transform Files (*.transform)|*.transform|All Files (*.*)|*.*";

		public ModelTransformationView()
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
				var context = (StructureRulesViewModel)DataContext;
				context.SaveTransforms(fileDialog.FileName);
			}
		}
	}
}
