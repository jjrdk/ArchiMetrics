// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RoslynExtensions.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the RoslynExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

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

		public static bool EquivalentTo(this SyntaxNode node1, SyntaxNode node2)
		{
			if (node1 == null || node2 == null)
			{
				return node1 == null && node2 == null;
			}

			return node1.RawKind == node2.RawKind && node1.ToFullString().Trim().Equals(node2.ToFullString().Trim());
		}
	}
}