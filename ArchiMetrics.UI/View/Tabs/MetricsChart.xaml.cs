namespace ArchiMetrics.UI.View.Tabs
{
	using System.Windows.Controls;
	using ArchiMetrics.UI.Support;
	using ArchiMetrics.UI.ViewModel;

	/// <summary>
	/// Interaction logic for MetricsChart.xaml.
	/// </summary>
	[DataContext(typeof(MetricsChartViewModel))]
	public partial class MetricsChart : UserControl
	{
		public MetricsChart()
		{
			InitializeComponent();
		}
	}
}
