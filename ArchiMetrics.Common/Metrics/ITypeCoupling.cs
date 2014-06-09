// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITypeCoupling.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ITypeCoupling type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Common.Metrics
{
	using System;
	using System.Collections.Generic;

	public interface ITypeCoupling : IComparable<ITypeCoupling>, ITypeDefinition
	{
		IEnumerable<string> UsedMethods { get; }

		IEnumerable<string> UsedProperties { get; }

		IEnumerable<string> UsedEvents { get; }
	}
}