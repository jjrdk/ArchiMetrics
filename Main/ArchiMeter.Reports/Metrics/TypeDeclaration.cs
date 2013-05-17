namespace ArchiMeter.Reports.Metrics
{
	using System.Collections.Generic;
	using Core.Metrics;

	public sealed class TypeDeclaration
	{
		// Properties
		public string Name { get; set; }

		public IEnumerable<TypeDeclarationSyntaxInfo> SyntaxNodes { get; set; }
	}
}