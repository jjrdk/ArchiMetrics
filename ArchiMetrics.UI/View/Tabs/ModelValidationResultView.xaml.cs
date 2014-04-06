namespace ArchiMetrics.UI.View.Tabs
{
	using System.Windows.Controls;
	using ArchiMetrics.UI.Support;
	using ArchiMetrics.UI.ViewModel;

	/// <summary>
	/// Interaction logic for ModelValidationResult.xaml.
	/// </summary>
	[DataContext(typeof(StructureRulesViewModel))]
	public partial class ModelValidationResultView : UserControl
	{
		public ModelValidationResultView()
		{
			InitializeComponent();
		}
	}
}
