// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IModelNode.cs" company="Reimers.dk">
//   Copyright © Matthias Friedrich, Reimers.dk 2014
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the IModelNode type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Common.Structure
{
    using System;
    using System.Collections.Generic;
    using CodeReview;

    /// <summary>
	/// Defines the interface for a node in an application model.
	/// </summary>
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