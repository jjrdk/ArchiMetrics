// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NamespaceCollectorSyntaxWalker.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the NamespaceCollectorSyntaxWalker type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMeter.CodeReview.Metrics
{
	using System.Collections.Generic;
	using System.Linq;
	using Roslyn.Compilers.Common;
	using Roslyn.Compilers.CSharp;

	internal sealed class NamespaceCollectorSyntaxWalker : SyntaxWalker
	{
		// Fields
		private readonly IList<NamespaceDeclarationSyntax> _namespaces;

		// Methods
		public NamespaceCollectorSyntaxWalker()
			: base(SyntaxWalkerDepth.Node)
		{
			_namespaces = new List<NamespaceDeclarationSyntax>();
		}

		public IEnumerable<T> GetNamespaces<T>(CommonSyntaxNode commonNode) where T : CommonSyntaxNode
		{
			//////Verify.NotNull<CommonSyntaxNode>(Expression.Lambda<Func<CommonSyntaxNode>>(Expression.Constant(commonNode), new ParameterExpression[0]), (string)null);
			var node = commonNode as SyntaxNode;
			if (node != null)
			{
				Visit(node);
			}

			return _namespaces.Cast<T>().ToList<T>().AsReadOnly();
		}

		public override void VisitNamespaceDeclaration(NamespaceDeclarationSyntax node)
		{
			base.VisitNamespaceDeclaration(node);
			_namespaces.Add(node);
		}
	}
}