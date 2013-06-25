namespace ArchiMetrics.Analysis.Metrics
{
	using System.Collections.Generic;
	using Roslyn.Compilers.CSharp;

	internal class SyntaxDeclarations
	{
		public IEnumerable<NamespaceDeclarationSyntax> NamespaceDeclarations { get; set; }

		public IEnumerable<TypeDeclarationSyntax> TypeDeclarations { get; set; }

		public IEnumerable<MemberDeclarationSyntax> MemberDeclarations { get; set; }
		
		public IEnumerable<StatementSyntax> Statements { get; set; }
	}
}
