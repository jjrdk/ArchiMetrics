// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyntaxCollector.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
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
	using Roslyn.Compilers;
	using Roslyn.Compilers.CSharp;

	internal sealed class SyntaxCollector : SyntaxWalker
	{
		private readonly IList<MemberDeclarationSyntax> _members = new List<MemberDeclarationSyntax>();
		private readonly IList<NamespaceDeclarationSyntax> _namespaces = new List<NamespaceDeclarationSyntax>();
		private readonly IList<StatementSyntax> _statements = new List<StatementSyntax>();
		private readonly IList<TypeDeclarationSyntax> _types = new List<TypeDeclarationSyntax>();

		public SyntaxDeclarations GetDeclarations(IEnumerable<SyntaxTree> trees)
		{
			var syntaxTrees = trees.ToArray();

			foreach (var root in syntaxTrees.Select(syntaxTree => syntaxTree.GetRoot()))
			{
				Visit(root);
				CheckStatementSyntax(root);
			}

			return new SyntaxDeclarations
			{
				MemberDeclarations = _members.ToArray(),
				NamespaceDeclarations = _namespaces.ToArray(),
				Statements = _statements.ToArray(),
				TypeDeclarations = _types.ToArray()
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
			var statements =
				node.ChildNodes()
					.Select(n => Syntax.ParseStatement(n.ToFullString(), options: new ParseOptions(kind: SourceCodeKind.Interactive, preprocessorSymbols: new string[0])))
					.Where(s => s.Kind != SyntaxKind.ExpressionStatement)
					.ToArray();

			foreach (var statement in statements)
			{
				_statements.Add(statement);
			}
		}
	}
}
