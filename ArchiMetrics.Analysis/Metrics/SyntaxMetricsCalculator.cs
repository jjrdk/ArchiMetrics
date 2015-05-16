// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyntaxMetricsCalculator.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the SyntaxMetricsCalculator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Metrics
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using ArchiMetrics.Common;
	using ArchiMetrics.Common.Metrics;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;

	public sealed class SyntaxMetricsCalculator
	{
		private readonly Func<SyntaxNode, bool> _isMethod = n => n.IsKind(SyntaxKind.MethodDeclaration) && (n as MethodDeclarationSyntax).Body != null;

		public IEnumerable<IHalsteadMetrics> Calculate(string code)
		{
			try
			{
				var tree = CSharpSyntaxTree.ParseText(code);
				var root = tree.GetRoot();
				var metrics = Calculate(root);
				return metrics;
			}
			catch
			{
				return new[] { new HalsteadMetrics(0, 0, 0, 0) };
			}
		}

		public IEnumerable<IHalsteadMetrics> Calculate(SyntaxNode root)
		{
			var analyzer = new HalsteadAnalyzer();
			var childNodes = root.ChildNodes().AsArray();

			var types = childNodes.Where(n => n.IsKind(SyntaxKind.ClassDeclaration) || n.IsKind(SyntaxKind.StructDeclaration))
				.AsArray();
			var methods = types.SelectMany(n => n.ChildNodes().Where(_isMethod));
			var getProperties = types.SelectMany(n => n.ChildNodes().Where(IsGetProperty));
			var setProperties = types.SelectMany(n => n.ChildNodes().Where(IsSetProperty));
			var looseMethods = childNodes.Where(_isMethod);
			var looseGetProperties = childNodes.Where(IsGetProperty);
			var looseSetProperties = childNodes.Where(IsSetProperty);
			var members = methods.Concat(getProperties)
								 .Concat(setProperties)
								 .Concat(looseMethods)
								 .Concat(looseGetProperties)
								 .Concat(looseSetProperties)
								 .AsArray();
			if (members.Any())
			{
				return members.Select(analyzer.Calculate);
			}

			var statements = childNodes.Length == 0
				? root.DescendantNodesAndTokens().Select(x => SyntaxFactory.ParseStatement(x.ToFullString(), 0, new CSharpParseOptions(kind: SourceCodeKind.Script, preprocessorSymbols: new string[0])))
				: childNodes.Select(x => SyntaxFactory.ParseStatement(x.ToFullString(), 0, new CSharpParseOptions(kind: SourceCodeKind.Script, preprocessorSymbols: new string[0])));

			var fakeMethod = SyntaxFactory.MethodDeclaration(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword)), "fake")
				.WithBody(SyntaxFactory.Block(statements));
			return new[]
				   {
					   analyzer.Calculate(fakeMethod)
				   };
		}

		private static bool IsGetProperty(SyntaxNode n)
		{
			if (!n.IsKind(SyntaxKind.PropertyDeclaration))
			{
				return false;
			}

			var propertyDeclarationSyntax = n as PropertyDeclarationSyntax;
			return propertyDeclarationSyntax != null && propertyDeclarationSyntax.AccessorList.Accessors.Any(a => a.IsKind(SyntaxKind.GetAccessorDeclaration));
		}

		private static bool IsSetProperty(SyntaxNode n)
		{
			if (!n.IsKind(SyntaxKind.PropertyDeclaration))
			{
				return false;
			}

			var propertyDeclarationSyntax = n as PropertyDeclarationSyntax;
			return propertyDeclarationSyntax != null && propertyDeclarationSyntax.AccessorList.Accessors.Any(a => a.IsKind(SyntaxKind.SetAccessorDeclaration));
		}
	}
}
