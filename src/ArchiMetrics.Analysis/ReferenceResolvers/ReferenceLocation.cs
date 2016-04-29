// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReferenceLocation.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ReferenceLocation type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.ReferenceResolvers
{
	using Microsoft.CodeAnalysis;

	public class ReferenceLocation
	{
		public ReferenceLocation(Location location, ITypeSymbol referencingType, SemanticModel model)
		{
			Location = location;
			ReferencingType = referencingType;
			Model = model;
		}

		public Location Location { get; private set; }

		public ITypeSymbol ReferencingType { get; private set; }

		public SemanticModel Model { get; private set; }
	}
}