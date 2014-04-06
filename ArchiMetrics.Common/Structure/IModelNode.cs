namespace ArchiMetrics.Common.Structure
{
	using System;
	using System.Collections.Generic;
	using ArchiMetrics.Common.CodeReview;

	public interface IModelNode : IEquatable<IModelNode>
	{
		IModelNode Parent { get; }

		string DisplayName { get; }

		string QualifiedName { get; }

		string Type { get; }

		CodeQuality Quality { get; }

		int LinesOfCode { get; }

		double MaintainabilityIndex { get; }

		int CyclomaticComplexity { get; }

		IEnumerable<IModelNode> Children { get; }

		void SetParent(IModelNode parent);

		IEnumerable<IModelNode> Flatten();

		void AddChild(IModelNode child);

		void RemoveChild(IModelNode child);
	}
}