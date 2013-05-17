namespace ArchiMeter.Reports.Metrics
{
	using System.Threading;

	using Roslyn.Compilers.CSharp;
	using Roslyn.Compilers.Common;

	internal static class SyntaxExtensions
	{
		public static string GetName(this NamespaceDeclarationSyntax node, CommonSyntaxNode rootNode)
		{
			NameSyntax name = node.Name;
			return rootNode.GetText().GetSubText(name.Span).ToString();
		}

		public static string GetName(this TypeDeclarationSyntax node, CommonSyntaxNode rootNode)
		{
			SyntaxToken identifier = node.Identifier;
			return rootNode.GetText().GetSubText(identifier.Span).ToString();
		}
	}
}