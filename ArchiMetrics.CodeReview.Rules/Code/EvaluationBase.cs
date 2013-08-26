// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EvaluationBase.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the EvaluationBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Rules.Code
{
	using System.Linq;
	using System.Text.RegularExpressions;
	using ArchiMetrics.Common.CodeReview;
	using Roslyn.Compilers.CSharp;

	internal abstract class EvaluationBase : IEvaluation
	{
		public abstract SyntaxKind EvaluatedKind { get; }
		
		public abstract string Title { get; }

		public abstract string Suggestion { get; }

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

		protected static SyntaxNode FindMethodParent(SyntaxNode node)
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

		protected static int GetLinesOfCode(string node)
		{
			return node.Split('\n').Count(s => Regex.IsMatch(s.Trim(), @"^(?!(\s*\/\/))\s*.{3,}"));
		}
	}
}