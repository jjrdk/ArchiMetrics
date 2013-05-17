namespace ArchiMeter.Reports.Metrics
{
	using System.Collections.Generic;
	using System.Linq;
	using Core.Metrics;
	using Roslyn.Compilers;
	using Roslyn.Compilers.CSharp;
	using Roslyn.Compilers.Common;

	internal sealed class MemberCollectorSyntaxWalker : SyntaxWalker
	{
		// Fields
		private readonly List<MemberNode> members;
		private readonly CommonSyntaxNode root;

		// Methods
		public MemberCollectorSyntaxWalker(CommonSyntaxNode root)
			: base(SyntaxWalkerDepth.Node)
		{
			this.members = new List<MemberNode>();
			this.root = root;
		}

		private void AddAccessorNode(SyntaxNode node, AccessorListSyntax accessorList, SyntaxKind filter, MemberKind kind)
		{
			if (accessorList.Accessors.SingleOrDefault(x => (x.Kind == filter)) != null)
			{
				MemberNode item = new MemberNode(string.Empty, string.Empty, kind, 0, node);
				this.members.Add(item);
			}
		}

		private int GetLineNumber(CommonSyntaxNode syntax)
		{
			if (syntax is MemberDeclarationSyntax)
			{
				IText text = this.root.GetText();
				text.GetSubText(syntax.Span);
				return text.GetLineNumberFromPosition(syntax.Span.Start);
			}
			return 0;
		}

		public IEnumerable<MemberNode> GetMembers(ISemanticModel semanticModel, TypeDeclarationSyntaxInfo type)
		{
			////Verify.NotNull<TypeDeclarationSyntaxInfo>(Expression.Lambda<Func<TypeDeclarationSyntaxInfo>>(Expression.Constant(type), new ParameterExpression[0]), (string)null);
			Visit((SyntaxNode)type.Syntax);
			var signatureResolver = new MemberNameResolver(semanticModel);
			return members.Select(x =>
									  {
										  string str;
										  signatureResolver.TryResolveMemberSignatureString(x, out str);
										  return new MemberNode(type.CodeFile, str, x.Kind, GetLineNumber(x.SyntaxNode), x.SyntaxNode);
									  })
						  .ToArray();
		}

		public override void VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
		{
			base.VisitConstructorDeclaration(node);
			MemberNode item = new MemberNode(string.Empty, string.Empty, MemberKind.Constructor, 0, node);
			this.members.Add(item);
		}

		public override void VisitDestructorDeclaration(DestructorDeclarationSyntax node)
		{
			base.VisitDestructorDeclaration(node);
			MemberNode item = new MemberNode(string.Empty, string.Empty, MemberKind.Destructor, 0, node);
			this.members.Add(item);
		}

		public override void VisitEventDeclaration(EventDeclarationSyntax node)
		{
			base.VisitEventDeclaration(node);
			this.AddAccessorNode(node, node.AccessorList, SyntaxKind.AddAccessorDeclaration, MemberKind.AddEventHandler);
			this.AddAccessorNode(node, node.AccessorList, SyntaxKind.RemoveAccessorDeclaration, MemberKind.RemoveEventHandler);
		}

		public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
		{
			base.VisitMethodDeclaration(node);
			MemberNode item = new MemberNode(string.Empty, string.Empty, MemberKind.Method, 0, node);
			this.members.Add(item);
		}

		public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
		{
			base.VisitPropertyDeclaration(node);
			this.AddAccessorNode(node, node.AccessorList, SyntaxKind.GetAccessorDeclaration, MemberKind.GetProperty);
			this.AddAccessorNode(node, node.AccessorList, SyntaxKind.SetAccessorDeclaration, MemberKind.SetProperty);
		}
	}
}