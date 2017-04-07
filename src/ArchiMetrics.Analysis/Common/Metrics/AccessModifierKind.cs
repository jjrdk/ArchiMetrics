// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AccessModifierKind.cs" company="Reimers.dk">
//   Copyright © Matthias Friedrich, Reimers.dk 2014
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the AccessModifierKind type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Common.Metrics
{
    using System;

    [Flags]
	public enum AccessModifierKind
	{
		Private = 1, 
		Protected = 2, 
		Public = 4, 
		Internal = 8
	}
}