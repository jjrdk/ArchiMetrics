// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StaticModelNode.cs" company="Reimers.dk">
//   Copyright © Matthias Friedrich, Reimers.dk 2014
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the StaticModelNode type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Model
{
	using System.Collections.Generic;
	using Common.CodeReview;
	using Common.Structure;

    internal class StaticModelNode : ModelNode
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