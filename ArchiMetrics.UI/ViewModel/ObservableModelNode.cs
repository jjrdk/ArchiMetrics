// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObservableModelNode.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ObservableModelNode type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

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