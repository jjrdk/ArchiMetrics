// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyntaxExtensions.cs" company="Reimers.dk">
//   Copyright © Matthias Friedrich, Reimers.dk 2014
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the SyntaxExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Metrics
{
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp.Syntax;

	internal static class SyntaxExtensions
	{
		public static string GetName(this NamespaceDeclarationSyntax node, SyntaxNode rootNode)
		{
			var name = node.Name;
			return rootNode.GetText().GetSubText(name.Span).ToString();
		}

		public static string GetName(this TypeDeclarationSyntax node, SyntaxNode rootNode)
		{
			var identifier = node.Identifier;
			return rootNode.GetText().GetSubText(identifier.Span).ToString();
		}
	}
}
