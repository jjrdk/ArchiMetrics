namespace ArchiMetrics.UI.View.Tabs
{
	using System.Windows.Controls;
	using ArchiMetrics.UI.Support;
	using ArchiMetrics.UI.ViewModel;

	/// <summary>
	/// Interaction logic for ErrorQualityAttributeChart.xaml.
	/// </summary>
	[DataContext(typeof(CodeErrorGraphViewModel))]
	public partial class ErrorQualityAttributeChart : UserControl
	{
		public ErrorQualityAttributeChart()
		{
			InitializeComponent();
		}
	}
}
