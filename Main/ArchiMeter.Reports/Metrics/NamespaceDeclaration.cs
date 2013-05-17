namespace ArchiMeter.Reports.Metrics
{
	using System.Collections.Generic;

	public sealed class NamespaceDeclaration
	{
		// Properties
		public string Name { get; set; }

		public IEnumerable<NamespaceDeclarationSyntaxInfo> SyntaxNodes { get; set; }
	}
}