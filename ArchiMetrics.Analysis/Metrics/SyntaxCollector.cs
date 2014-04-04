// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyntaxCollector.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
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
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;

	internal sealed class SyntaxCollector : SyntaxWalker
	{
		private static readonly Type[] TypeDeclarations = new[]
														  {
															  typeof(ClassDeclarationSyntax), 
															  typeof(StructDeclarationSyntax), 
															  typeof(InterfaceDeclarationSyntax)
														  };
		private static readonly Type[] NamespaceDeclarations = new[]
														  {
															  typeof(NamespaceDeclarationSyntax)
														  };
		private static readonly Type[] MemberDeclarations = new[]
														  {
															  typeof(ConstructorDeclarationSyntax),
															  typeof(DestructorDeclarationSyntax),
															  typeof(EventDeclarationSyntax),
															  typeof(MethodDeclarationSyntax),
															  typeof(PropertyDeclarationSyntax)
														  };
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

		/// <summary>
		/// Called when the walker visits a node.  This method may be overridden if subclasses want
		///             to handle the node.  Overrides should call back into this base method if they want the
		///             children of this node to be visited.
		/// </summary>
		/// <param name="node">The current node that the walker is visiting.</param>
		public override void Visit(SyntaxNode node)
		{
			var typeDeclaration = node as TypeDeclarationSyntax;
			if (typeDeclaration != null)
			{
				_types.Add(typeDeclaration);
			}
			else
			{
				var namespaceDeclaration = node as NamespaceDeclarationSyntax;
				if (namespaceDeclaration != null)
				{
					_namespaces.Add(namespaceDeclaration);
				}
				else
				{
					var memberDeclaration = node as MemberDeclarationSyntax;
					if (memberDeclaration != null)
					{
						_members.Add(memberDeclaration);
					}
				}
			}
			base.Visit(node);
		}

		private void CheckStatementSyntax(SyntaxNode node)
		{
			var statements =
				node.ChildNodes()
					.Select(n => SyntaxFactory.ParseStatement(n.ToFullString(), options: new CSharpParseOptions(kind: SourceCodeKind.Interactive, preprocessorSymbols: new string[0])))
					.Where(s => !s.IsKind(SyntaxKind.ExpressionStatement))
					.ToArray();

			foreach (var statement in statements)
			{
				_statements.Add(statement);
			}
		}
	}
}
