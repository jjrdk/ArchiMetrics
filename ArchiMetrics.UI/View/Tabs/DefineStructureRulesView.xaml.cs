namespace ArchiMetrics.UI.View.Tabs
{
	using System.Windows.Controls;
	using ArchiMetrics.UI.Support;
	using ArchiMetrics.UI.ViewModel;

	/// <summary>
	/// Interaction logic for DefineStructureRulesView.xaml.
	/// </summary>
	[DataContext(typeof(StructureRulesViewModel))]
	public partial class DefineStructureRulesView : UserControl
	{
		public DefineStructureRulesView()
		{
			InitializeComponent();
		}
	}
}
