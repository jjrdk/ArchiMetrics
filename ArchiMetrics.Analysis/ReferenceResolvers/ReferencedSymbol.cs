// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReferencedSymbol.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ReferencedSymbol type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.ReferenceResolvers
{
	using System.Collections.Generic;
	using System.Linq;
	using ArchiMetrics.Common;
	using Microsoft.CodeAnalysis;

	public class ReferencedSymbol
	{
		public ReferencedSymbol(ISymbol symbol, IEnumerable<ReferenceLocation> locations)
		{
			Symbol = symbol;
			Locations = locations.AsArray();
		}

		public ISymbol Symbol { get; private set; }

		public IEnumerable<ReferenceLocation> Locations { get; private set; }
	}
}