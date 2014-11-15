namespace ArchiMetrics.Analysis.ReferenceResolvers
{
	using System.Collections.Generic;
	using System.Linq;
	using Microsoft.CodeAnalysis;

	public class ReferencedSymbol
	{
		public ReferencedSymbol(ISymbol symbol, IEnumerable<Location> locations)
		{
			Symbol = symbol;
			Locations = locations.ToArray();
		}

		public ISymbol Symbol { get; private set; }

		public IEnumerable<Location> Locations { get; private set; }
	}
}