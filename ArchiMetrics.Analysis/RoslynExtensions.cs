namespace ArchiMetrics.Analysis
{
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;

	public static class RoslynExtensions
	{
		public static MethodDeclarationSyntax GetMethod(this SyntaxToken token)
		{
			var parent = token.Parent;
			return GetMethod(parent);
		}

		public static MethodDeclarationSyntax GetMethod(this SyntaxNode node)
		{
			if (node == null)
			{
				return null;
			}

			if (node.IsKind(SyntaxKind.MethodDeclaration))
			{
				return (MethodDeclarationSyntax)node;
			}

			return GetMethod(node.Parent);
		}
	}
}