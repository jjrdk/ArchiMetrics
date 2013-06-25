// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyntaxExtensions.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the SyntaxExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMetrics.Analysis.Metrics
{
	using Roslyn.Compilers.Common;
	using Roslyn.Compilers.CSharp;

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
