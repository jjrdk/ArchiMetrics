namespace ArchiMetrics.UI.View.Tabs
{
	using System.Windows.Controls;
	using ArchiMetrics.UI.Support;
	using ArchiMetrics.UI.ViewModel;

	/// <summary>
	/// Interaction logic for CodeErrorGraphView.xaml.
	/// </summary>
	[DataContext(typeof(CodeErrorGraphViewModel))]
	public partial class CodeErrorGraphView : UserControl
	{
		public CodeErrorGraphView()
		{
			InitializeComponent();
		}
	}
}
