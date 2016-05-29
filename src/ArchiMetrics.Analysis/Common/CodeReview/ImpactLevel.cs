// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImpactLevel.cs" company="Reimers.dk">
//   Copyright © Matthias Friedrich, Reimers.dk 2014
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ImpactLevel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Common.CodeReview
{
	public enum ImpactLevel
	{
		Project = 0, 
		Namespace = 1, 
		Type = 2, 
		Member = 3, 
		Line = 4, 
		Node = 5
	}
}
