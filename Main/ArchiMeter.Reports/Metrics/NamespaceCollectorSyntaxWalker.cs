namespace ArchiMeter.Reports.Metrics
{
	using System.Collections.Generic;
	using System.Linq;
	using Roslyn.Compilers.CSharp;
	using Roslyn.Compilers.Common;

	internal sealed class NamespaceCollectorSyntaxWalker : SyntaxWalker
	{
		private readonly IList<NamespaceDeclarationSyntax> _namespaces;

		public NamespaceCollectorSyntaxWalker()
			: base(SyntaxWalkerDepth.Node)
		{
			this._namespaces = new List<NamespaceDeclarationSyntax>();
		}

		public IEnumerable<T> GetNamespaces<T>(CommonSyntaxNode commonNode) where T : CommonSyntaxNode
		{
			var node = commonNode as SyntaxNode;
			if (node != null)
			{
				this.Visit(node);
			}
			return _namespaces.Cast<T>().ToArray();
		}

		public override void VisitNamespaceDeclaration(NamespaceDeclarationSyntax node)
		{
			base.VisitNamespaceDeclaration(node);
			this._namespaces.Add(node);
		}
	}
}