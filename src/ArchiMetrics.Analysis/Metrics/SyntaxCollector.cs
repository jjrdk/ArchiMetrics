// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyntaxCollector.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the SyntaxCollector type.
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

	internal sealed class SyntaxCollector : CSharpSyntaxWalker
	{
		private readonly IList<MemberDeclarationSyntax> _members = new List<MemberDeclarationSyntax>();
		private readonly IList<NamespaceDeclarationSyntax> _namespaces = new List<NamespaceDeclarationSyntax>();
		private readonly IList<SyntaxNode> _statements = new List<SyntaxNode>();
		private readonly IList<TypeDeclarationSyntax> _types = new List<TypeDeclarationSyntax>();

		public SyntaxDeclarations GetDeclarations(IEnumerable<SyntaxTree> trees)
		{
			var syntaxTrees = trees.AsArray();

			foreach (var root in syntaxTrees.Select(syntaxTree => syntaxTree.GetRoot()))
			{
				Visit(root);
				CheckStatementSyntax(root);
			}

			return new SyntaxDeclarations
			{
				MemberDeclarations = _members.AsArray(),
				NamespaceDeclarations = _namespaces.AsArray(),
				Statements = _statements.AsArray(),
				TypeDeclarations = _types.AsArray()
			};
		}

		public override void VisitNamespaceDeclaration(NamespaceDeclarationSyntax node)
		{
			_namespaces.Add(node);
		}

		public override void VisitClassDeclaration(ClassDeclarationSyntax node)
		{
			_types.Add(node);
		}

		public override void VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
		{
			_types.Add(node);
		}

		public override void VisitStructDeclaration(StructDeclarationSyntax node)
		{
			_types.Add(node);
		}

		public override void VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
		{
			_members.Add(node);
		}

		public override void VisitDestructorDeclaration(DestructorDeclarationSyntax node)
		{
			_members.Add(node);
		}

		public override void VisitEventDeclaration(EventDeclarationSyntax node)
		{
			_members.Add(node);
		}

		public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
		{
			_members.Add(node);
		}

		public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
		{
			_members.Add(node);
		}

		private void CheckStatementSyntax(SyntaxNode node)
		{
			var syntaxNodes = node.ChildNodes().AsArray();

			var statements =
				syntaxNodes
				.Where(x => !(x is TypeDeclarationSyntax))
					.Where(x => x is BaseFieldDeclarationSyntax || x is StatementSyntax)
					.AsArray();

			foreach (var statement in statements)
			{
				_statements.Add(statement);
			}
		}
	}
}
