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