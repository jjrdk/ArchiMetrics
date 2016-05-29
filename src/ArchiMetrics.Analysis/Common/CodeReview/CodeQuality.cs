// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CodeQuality.cs" company="Reimers.dk">
//   Copyright © Matthias Friedrich, Reimers.dk 2014
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the CodeQuality type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Common.CodeReview
{
	public enum CodeQuality
	{
		Broken = 0,
		NeedsReEngineering = 1,
		NeedsRefactoring = 2,
		NeedsCleanup = 3,
		NeedsReview = 4,
		Good = 5
	}
}
