// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AccessModifierKind.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
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