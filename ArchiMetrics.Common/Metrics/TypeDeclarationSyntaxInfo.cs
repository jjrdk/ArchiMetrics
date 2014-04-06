namespace ArchiMetrics.Common.Metrics
{
	using Microsoft.CodeAnalysis.CSharp.Syntax;

	public class TypeDeclarationSyntaxInfo
	{
		public TypeDeclarationSyntaxInfo(string codeFile, string name, TypeDeclarationSyntax syntax)
		{
			CodeFile = codeFile;
			Name = name;
			Syntax = syntax;
		}

		public string CodeFile { get; private set; }

		public string Name { get; private set; }

		public TypeDeclarationSyntax Syntax { get; set; }
	}
}
