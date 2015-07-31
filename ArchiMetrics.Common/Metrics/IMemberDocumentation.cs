// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMemberDocumentation.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the IMemberDocumentation type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Common.Metrics
{
	using System.Collections.Generic;

	public interface IMemberDocumentation : IDocumentation
	{
		IEnumerable<ParameterDocumentation> Parameters { get; }

		IEnumerable<TypeParameterDocumentation> TypeParameters { get; }

		IEnumerable<ExceptionDocumentation> Exceptions { get; }
	}
}