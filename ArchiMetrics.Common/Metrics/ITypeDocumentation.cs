// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITypeDocumentation.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ITypeDocumentation type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Common.Metrics
{
	using System.Collections.Generic;

	public interface ITypeDocumentation : IDocumentation
	{
		IEnumerable<TypeParameterDocumentation> TypeParameters { get; } 
	}
}