// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MemberCollectorSyntaxWalker.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the MemberCollectorSyntaxWalker type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMeter.CodeReview.Metrics
{
	using System.Collections.Generic;
	using System.Linq;
	using Common.Metrics;
	using Roslyn.Compilers;
	using Roslyn.Compilers.CSharp;
	using Roslyn.Compilers.Common;

	internal sealed class MemberCollectorSyntaxWalker : SyntaxWalker
	{
		// Fields
		private readonly List<MemberNode> _members;
		private readonly CommonSyntaxNode _root;

		// Methods
		public MemberCollectorSyntaxWalker(CommonSyntaxNode root)
			: base(SyntaxWalkerDepth.Node)
		{
			_members = new List<MemberNode>();
			_root = root;
		}

		public IEnumerable<MemberNode> GetMembers(ISemanticModel semanticModel, TypeDeclarationSyntaxInfo type)
		{
			Visit((SyntaxNode)type.Syntax);
			var signatureResolver = new MemberNameResolver(semanticModel);
			return _members.Select(x => new MemberNode(
				                            type.CodeFile, 
				                            signatureResolver.TryResolveMemberSignatureString(x), 
				                            x.Kind, 
				                            GetLineNumber(x.SyntaxNode), 
				                            x.SyntaxNode));
		}

		public override void VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
		{
			base.VisitConstructorDeclaration(node);
			var item = new MemberNode(string.Empty, string.Empty, MemberKind.Constructor, 0, node);
			_members.Add(item);
		}

		public override void VisitDestructorDeclaration(DestructorDeclarationSyntax node)
		{
			base.VisitDestructorDeclaration(node);
			var item = new MemberNode(string.Empty, string.Empty, MemberKind.Destructor, 0, node);
			_members.Add(item);
		}

		public override void VisitEventDeclaration(EventDeclarationSyntax node)
		{
			base.VisitEventDeclaration(node);
			AddAccessorNode(node, node.AccessorList, SyntaxKind.AddAccessorDeclaration, MemberKind.AddEventHandler);
			AddAccessorNode(node, node.AccessorList, SyntaxKind.RemoveAccessorDeclaration, MemberKind.RemoveEventHandler);
		}

		public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
		{
			base.VisitMethodDeclaration(node);
			var item = new MemberNode(string.Empty, string.Empty, MemberKind.Method, 0, node);
			_members.Add(item);
		}

		public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
		{
			base.VisitPropertyDeclaration(node);
			AddAccessorNode(node, node.AccessorList, SyntaxKind.GetAccessorDeclaration, MemberKind.GetProperty);
			AddAccessorNode(node, node.AccessorList, SyntaxKind.SetAccessorDeclaration, MemberKind.SetProperty);
		}

		private void AddAccessorNode(SyntaxNode node, AccessorListSyntax accessorList, SyntaxKind filter, MemberKind kind)
		{
			if (accessorList.Accessors.Any(x => (x.Kind == filter)))
			{
				var item = new MemberNode(string.Empty, string.Empty, kind, 0, node);
				_members.Add(item);
			}
		}

		private int GetLineNumber(CommonSyntaxNode syntax)
		{
			if (syntax is MemberDeclarationSyntax)
			{
				IText text = _root.GetText();
				text.GetSubText(syntax.Span);
				return text.GetLineNumberFromPosition(syntax.Span.Start);
			}

			return 0;
		}
	}
}