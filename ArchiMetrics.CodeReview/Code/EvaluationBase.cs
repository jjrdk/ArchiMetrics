namespace ArchiMetrics.CodeReview.Rules
{
	using System.Linq;
	using System.Text.RegularExpressions;
	using Roslyn.Compilers.CSharp;

	internal abstract class EvaluationBase : IEvaluation
	{
		public abstract SyntaxKind EvaluatedKind { get; }

		protected static string GetCompilationUnitNamespace(CompilationUnitSyntax node)
		{
			var namespaceDeclaration = node.DescendantNodes()
				.FirstOrDefault(n => n.Kind == SyntaxKind.NamespaceDeclaration);

			return namespaceDeclaration == null ? string.Empty : ((NamespaceDeclarationSyntax)namespaceDeclaration).Name.GetText().ToString().Trim();
		}

		protected TypeDeclarationSyntax FindClassParent(SyntaxNode node)
		{
			if (node.Parent == null)
			{
				return null;
			}

			if (node.Parent.Kind == SyntaxKind.ClassDeclaration || node.Parent.Kind == SyntaxKind.StructDeclaration)
			{
				return node.Parent as TypeDeclarationSyntax;
			}

			return FindClassParent(node.Parent);
		}

		protected SyntaxNode FindMethodParent(SyntaxNode node)
		{
			if (node.Parent == null)
			{
				return null;
			}

			if (node.Parent.Kind == SyntaxKind.MethodDeclaration || node.Parent.Kind == SyntaxKind.ConstructorDeclaration)
			{
				return node.Parent;
			}

			return FindMethodParent(node.Parent);
		}

		protected int GetLinesOfCode(string node)
		{
			return node.Split('\n').Count(s => Regex.IsMatch(s.Trim(), @"^(?!(\s*\/\/))\s*.{3,}"));
		}
	}
}