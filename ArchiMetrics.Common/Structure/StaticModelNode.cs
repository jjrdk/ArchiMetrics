// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StaticModelNode.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the StaticModelNode type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

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