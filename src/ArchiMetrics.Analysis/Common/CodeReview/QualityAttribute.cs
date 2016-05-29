// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QualityAttribute.cs" company="Reimers.dk">
//   Copyright © Matthias Friedrich, Reimers.dk 2014
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the QualityAttribute type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Common.CodeReview
{
    using System;

    [Flags]
	public enum QualityAttribute
	{
		CodeQuality = 1, 
		Maintainability = 2, 
		Testability = 4, 
		Modifiability = 8, 
		Reusability = 16, 
		Conformance = 32, 
		Security = 64,
		Performance = 128
	}
}
