// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITypeDefinition.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ITypeDefinition type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Common.Metrics
{
	using System;

	public interface ITypeDefinition : IComparable<ITypeDefinition>
	{
		string TypeName { get; }

		string Namespace { get; }

		string Assembly { get; }
	}
}