// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReferencedSymbol.cs" company="Reimers.dk">
//   Copyright © Matthias Friedrich, Reimers.dk 2014
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ReferencedSymbol type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.ReferenceResolvers
{
	using System.Collections.Generic;
	using Common;
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