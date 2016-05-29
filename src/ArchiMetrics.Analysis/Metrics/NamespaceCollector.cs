// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NamespaceCollector.cs" company="Reimers.dk">
//   Copyright © Matthias Friedrich, Reimers.dk 2014
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
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
	using Common;
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

			return _namespaces.AsArray();
		}

		public override void VisitNamespaceDeclaration(NamespaceDeclarationSyntax node)
		{
			base.VisitNamespaceDeclaration(node);
			_namespaces.Add(node);
		}
	}
}
