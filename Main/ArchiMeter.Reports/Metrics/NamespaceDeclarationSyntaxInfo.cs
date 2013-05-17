namespace ArchiMeter.Reports.Metrics
{
	using Roslyn.Compilers.Common;

	public sealed class NamespaceDeclarationSyntaxInfo
	{
		// Properties
		public string CodeFile { get; set; }

		public string Name { get; set; }

		public CommonSyntaxNode Syntax { get; set; }
	}
}