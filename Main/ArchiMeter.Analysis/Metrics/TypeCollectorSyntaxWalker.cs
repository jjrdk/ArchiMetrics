// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeCollectorSyntaxWalker.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the TypeCollectorSyntaxWalker type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMeter.Analysis.Metrics
{
	using System.Collections.Generic;
	using System.Linq;
	using Roslyn.Compilers.Common;
	using Roslyn.Compilers.CSharp;

	internal sealed class TypeCollectorSyntaxWalker : SyntaxWalker
	{
		// Fields
		private readonly IList<TypeDeclarationSyntax> _types;

		// Methods
		public TypeCollectorSyntaxWalker()
			: base(SyntaxWalkerDepth.Node)
		{
			_types = new List<TypeDeclarationSyntax>();
		}

		public IEnumerable<T> GetTypes<T>(CommonSyntaxNode namespaceNode) where T : CommonSyntaxNode
		{
			var node = namespaceNode as NamespaceDeclarationSyntax;
			if (node != null)
			{
				Visit(node);
			}

			return _types.OfType<T>().ToArray();
		}

		public override void VisitClassDeclaration(ClassDeclarationSyntax node)
		{
			base.VisitClassDeclaration(node);
			_types.Add(node);
		}

		public override void VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
		{
			base.VisitInterfaceDeclaration(node);
			_types.Add(node);
		}

		public override void VisitStructDeclaration(StructDeclarationSyntax node)
		{
			base.VisitStructDeclaration(node);
			_types.Add(node);
		}
	}
}