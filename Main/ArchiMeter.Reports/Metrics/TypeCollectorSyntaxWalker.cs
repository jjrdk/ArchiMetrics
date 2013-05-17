namespace ArchiMeter.Reports.Metrics
{
	using System.Collections.Generic;
	using System.Linq;

	using Roslyn.Compilers.CSharp;
	using Roslyn.Compilers.Common;

	internal sealed class TypeCollectorSyntaxWalker : SyntaxWalker
	{
		private readonly IList<TypeDeclarationSyntax> _types;

		public TypeCollectorSyntaxWalker()
			: base(SyntaxWalkerDepth.Node)
		{
			this._types = new List<TypeDeclarationSyntax>();
		}

		public IEnumerable<T> GetTypes<T>(CommonSyntaxNode namespaceNode) where T : CommonSyntaxNode
		{
			var node = namespaceNode as NamespaceDeclarationSyntax;
			if (node != null)
			{
				this.Visit(node);
			}
			return this._types.OfType<T>().ToArray();
		}

		public override void VisitClassDeclaration(ClassDeclarationSyntax node)
		{
			base.VisitClassDeclaration(node);
			this._types.Add(node);
		}

		public override void VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
		{
			base.VisitInterfaceDeclaration(node);
			this._types.Add(node);
		}

		public override void VisitStructDeclaration(StructDeclarationSyntax node)
		{
			base.VisitStructDeclaration(node);
			this._types.Add(node);
		}
	}
}