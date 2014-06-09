// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NamespaceCollector.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the NamespaceCollector type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Metrics
{
	using System.Collections.Generic;
	using System.Linq;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;

	internal sealed class NamespaceCollector : CSharpSyntaxWalker
	{
		private readonly IList<NamespaceDeclarationSyntax> _namespaces;

		public NamespaceCollector()
			: base(SyntaxWalkerDepth.Node)
		{
			_namespaces = new List<NamespaceDeclarationSyntax>();
		}

		public IEnumerable<NamespaceDeclarationSyntax> GetNamespaces(SyntaxNode commonNode)
		{
			var node = commonNode as SyntaxNode;
			if (node != null)
			{
				Visit(node);
			}

			return _namespaces.ToArray();
		}

		public override void VisitNamespaceDeclaration(NamespaceDeclarationSyntax node)
		{
			base.VisitNamespaceDeclaration(node);
			_namespaces.Add(node);
		}
	}
}
