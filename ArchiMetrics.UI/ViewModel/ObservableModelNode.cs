namespace ArchiMetrics.UI.ViewModel
{
	using System.Collections.ObjectModel;
	using ArchiMetrics.Common.CodeReview;
	using ArchiMetrics.Common.Structure;

	internal class ObservableModelNode : ModelNode
	{
		public ObservableModelNode(string name, string type, CodeQuality quality, int linesOfCode, double maintainabilityIndex, int cyclomaticComplexity)
			: base(name, type, quality, linesOfCode, maintainabilityIndex, cyclomaticComplexity, new ObservableCollection<IModelNode>())
		{
		}
	}
}