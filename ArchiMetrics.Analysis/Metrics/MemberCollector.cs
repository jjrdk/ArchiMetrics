// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MemberCollector.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the MemberCollector type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Metrics
{
	using System.Collections.Generic;
	using System.Linq;
	using ArchiMetrics.Common.Metrics;
	using Roslyn.Compilers.Common;
	using Roslyn.Compilers.CSharp;

	internal sealed class MemberCollector : SyntaxWalker
	{
		private readonly List<SyntaxNode> _members;
		private readonly CommonSyntaxNode _root;

		public MemberCollector(CommonSyntaxNode root)
			: base(SyntaxWalkerDepth.Node)
		{
			_members = new List<SyntaxNode>();
			_root = root;
		}

		public IEnumerable<SyntaxNode> GetMembers(TypeDeclarationSyntaxInfo type)
		{
			Visit((SyntaxNode)type.Syntax);
			return _members.ToList();
		}

		public override void VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
		{
			base.VisitConstructorDeclaration(node);
			_members.Add(node);
		}

		public override void VisitDestructorDeclaration(DestructorDeclarationSyntax node)
		{
			base.VisitDestructorDeclaration(node);
			_members.Add(node);
		}

		public override void VisitEventDeclaration(EventDeclarationSyntax node)
		{
			base.VisitEventDeclaration(node);
			AddAccessorNode(node, node.AccessorList, SyntaxKind.AddAccessorDeclaration);
			AddAccessorNode(node, node.AccessorList, SyntaxKind.RemoveAccessorDeclaration);
		}

		public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
		{
			base.VisitMethodDeclaration(node);
			_members.Add(node);
		}

		public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
		{
			base.VisitPropertyDeclaration(node);
			AddAccessorNode(node, node.AccessorList, SyntaxKind.GetAccessorDeclaration);
			AddAccessorNode(node, node.AccessorList, SyntaxKind.SetAccessorDeclaration);
		}

		private void AddAccessorNode(SyntaxNode node, AccessorListSyntax accessorList, SyntaxKind filter)
		{
			if (accessorList.Accessors.Any(x => (x.Kind == filter)))
			{
				_members.Add(node);
			}
		}
	}
}
