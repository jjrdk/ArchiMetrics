// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EvaluationBase.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the EvaluationBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Rules.Code
{
	using System;
	using ArchiMetrics.Analysis.Metrics;
	using ArchiMetrics.Common;
	using ArchiMetrics.Common.CodeReview;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;

	internal abstract class EvaluationBase : IEvaluation
	{
		private readonly LinesOfCodeCalculator _locCalculator = new LinesOfCodeCalculator();

		public abstract CodeQuality Quality { get; }

		public abstract QualityAttribute QualityAttribute { get; }

		public abstract ImpactLevel ImpactLevel { get; }

		public abstract SyntaxKind EvaluatedKind { get; }

		public abstract string Title { get; }

		public abstract string Suggestion { get; }

		protected static string GetNamespace(SyntaxNode node)
		{
			var namespaceDeclaration = node as NamespaceDeclarationSyntax;
			if (namespaceDeclaration != null)
			{
				return namespaceDeclaration.Name.GetText().ToString().Trim();
			}

			if (node.Parent == null)
			{
				return SyntaxFactory.Token(SyntaxKind.GlobalKeyword).ValueText;
			}

			return GetNamespace(node.Parent);
		}

		protected static Tuple<string, string> GetNodeType(SyntaxNode node)
		{
			var declaration = node as TypeDeclarationSyntax;
			if (declaration != null)
			{
				var keyword = declaration.Keyword.ValueText.ToTitleCase();
				var name = declaration.Identifier.ValueText;
				return new Tuple<string, string>(keyword, name);
			}

			if (node.Parent == null)
			{
				return new Tuple<string, string>(SyntaxFactory.Token(SyntaxKind.GlobalKeyword).ValueText, string.Empty);
			}

			return GetNodeType(node.Parent);
		}

		protected static SyntaxNode FindMethodParent(SyntaxNode node)
		{
			if (node.Parent == null)
			{
				return null;
			}

			if (node.Parent.IsKind(SyntaxKind.MethodDeclaration) || node.Parent.IsKind(SyntaxKind.ConstructorDeclaration))
			{
				return node.Parent;
			}

			return FindMethodParent(node.Parent);
		}

		protected int GetLinesOfCode(SyntaxNode node)
		{
			return _locCalculator.Calculate(node);
		}

		protected TypeDeclarationSyntax FindClassParent(SyntaxNode node)
		{
			if (node.Parent == null)
			{
				return null;
			}

			if (node.Parent.IsKind(SyntaxKind.ClassDeclaration) || node.Parent.IsKind(SyntaxKind.StructDeclaration))
			{
				return node.Parent as TypeDeclarationSyntax;
			}

			return FindClassParent(node.Parent);
		}

		protected NamespaceDeclarationSyntax FindNamespaceParent(SyntaxNode node)
		{
			if (node.Parent == null)
			{
				return null;
			}

			if (node.Parent.IsKind(SyntaxKind.NamespaceDeclaration))
			{
				return node.Parent as NamespaceDeclarationSyntax;
			}

			return FindNamespaceParent(node.Parent);
		}
	}
}