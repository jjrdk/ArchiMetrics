namespace ArchiMetrics.Common.Structure
{
	using System.Collections.Generic;
	using ArchiMetrics.Common.CodeReview;

	public class StaticModelNode : ModelNode
	{
		public StaticModelNode(string name, string type, CodeQuality quality, int linesOfCode, double maintainabilityIndex, int cyclomaticComplexity, IList<IModelNode> children)
			: base(name, type, quality, linesOfCode, maintainabilityIndex, cyclomaticComplexity, children)
		{
		}

		public override void SetParent(IModelNode parent)
		{
			Parent = parent;
		}
	}
}