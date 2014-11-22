namespace ArchiMetrics.Analysis.ReferenceResolvers
{
	using Microsoft.CodeAnalysis;

	public class ReferenceLocation
	{
		public ReferenceLocation(Location location, SemanticModel model)
		{
			Location = location;
			Model = model;
		}

		public Location Location { get; private set; }
	
		public SemanticModel Model { get; private set; }
	}
}