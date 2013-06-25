// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CodeEvaluationBase.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the CodeEvaluationBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMetrics.CodeReview.Rules
{
	using System.Linq;
	using System.Text.RegularExpressions;
	using Common;
	using Roslyn.Compilers.CSharp;

	internal abstract class CodeEvaluationBase : ICodeEvaluation
	{
		public EvaluationResult Evaluate(SyntaxNode node)
		{
			var result = EvaluateImpl(node);
			if (result != null)
			{
				var sourceTree = node.GetLocation().SourceTree;
				var filePath = sourceTree.FilePath;
				var unitNamespace = GetCompilationUnitNamespace(sourceTree.GetRoot());
				result.Namespace = unitNamespace;
				result.FilePath = filePath;
				result.LinesOfCodeAffected = GetLinesOfCode(result.Snippet);
			}

			return result;
		}

		public abstract SyntaxKind EvaluatedKind { get; }

		protected abstract EvaluationResult EvaluateImpl(SyntaxNode node);

		private static string GetCompilationUnitNamespace(CompilationUnitSyntax node)
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
