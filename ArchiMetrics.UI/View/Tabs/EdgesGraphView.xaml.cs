namespace ArchiMetrics.UI.View.Tabs
{
	using System.Windows.Controls;
	using ArchiMetrics.UI.Support;
	using ArchiMetrics.UI.ViewModel;

	/// <summary>
	/// Interaction logic for GraphView.xaml.
	/// </summary>
	[DataContext(typeof(GraphViewModel))]
	public partial class EdgesGraphView : UserControl
	{
		public EdgesGraphView()
		{
			InitializeComponent();
		}
	}
}
