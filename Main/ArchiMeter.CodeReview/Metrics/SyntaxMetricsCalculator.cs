// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyntaxMetricsCalculator.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the SyntaxMetricsCalculator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMeter.CodeReview.Metrics
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Common.Metrics;
	using Roslyn.Compilers;
	using Roslyn.Compilers.CSharp;

	public sealed class SyntaxMetricsCalculator
	{
		private readonly Func<SyntaxNode, bool> _isGetProperty = n => n.Kind == SyntaxKind.PropertyDeclaration && (n as PropertyDeclarationSyntax).AccessorList.Accessors.Any(a => a.Kind == SyntaxKind.GetAccessorDeclaration);
		private readonly Func<SyntaxNode, bool> _isMethod = n => n.Kind == SyntaxKind.MethodDeclaration && (n as MethodDeclarationSyntax).Body != null;
		private readonly Func<SyntaxNode, bool> _isSetProperty = n => n.Kind == SyntaxKind.PropertyDeclaration && (n as PropertyDeclarationSyntax).AccessorList.Accessors.Any(a => a.Kind == SyntaxKind.SetAccessorDeclaration);

		public IEnumerable<IHalsteadMetrics> Calculate(string code)
		{
			try
			{
				var tree = SyntaxTree.ParseText(code);
				var root = tree.GetRoot();
				var metrics = Calculate(root);

				return metrics;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return new[] { new HalsteadMetrics(0, 0, 0, 0) };
			}
		}

		public IEnumerable<IHalsteadMetrics> Calculate(CompilationUnitSyntax root)
		{
			var analyzer = new HalsteadAnalyzer();
			var childNodes = root.ChildNodes().ToArray();

			var types = childNodes.Where(n => n.Kind == SyntaxKind.ClassDeclaration || n.Kind == SyntaxKind.StructDeclaration)
				.ToArray();
			var methods = types.SelectMany(n => n.ChildNodes().Where(_isMethod))
				.Select(n => CreateMemberNode(MemberKind.Method, n));
			var getProperties = types.SelectMany(n => n.ChildNodes().Where(_isGetProperty))
				.Select(n => CreateMemberNode(MemberKind.GetProperty, n));
			var setProperties = types.SelectMany(n => n.ChildNodes().Where(_isSetProperty))
				.Select(n => CreateMemberNode(MemberKind.SetProperty, n));
			var looseMethods = childNodes.Where(_isMethod)
				.Select(n => CreateMemberNode(MemberKind.Method, n));
			var looseGetProperties = childNodes.Where(_isGetProperty)
				.Select(n => CreateMemberNode(MemberKind.GetProperty, n));
			var looseSetProperties = childNodes.Where(_isSetProperty)
				.Select(n => CreateMemberNode(MemberKind.SetProperty, n));
			var members = methods.Concat(getProperties)
								 .Concat(setProperties)
								 .Concat(looseMethods)
								 .Concat(looseGetProperties)
								 .Concat(looseSetProperties)
								 .ToArray();
			if (!members.Any())
			{
				var statements = childNodes.Length == 0
					? root.DescendantNodesAndTokens().Select(x => Syntax.ParseStatement(x.ToFullString(), 0, new ParseOptions(kind: SourceCodeKind.Script, preprocessorSymbols: new string[0])))
					: childNodes.Select(x => Syntax.ParseStatement(x.ToFullString(), 0, new ParseOptions(kind: SourceCodeKind.Script, preprocessorSymbols: new string[0])));

				var fakeMethod = Syntax.MethodDeclaration(Syntax.PredefinedType(Syntax.Token(SyntaxKind.VoidKeyword)), "fake")
									   .WithBody(Syntax.Block(statements));
				return new[]
					       {
						       analyzer.Calculate(
							       new MemberNode(
							       string.Empty, 
							       string.Empty, 
							       MemberKind.Method, 
							       0, 
							       fakeMethod))
					       };
			}

			return members.Select(analyzer.Calculate);
		}

		private MemberNode CreateMemberNode(MemberKind kind, SyntaxNode node)
		{
			return new MemberNode(string.Empty, string.Empty, kind, 0, node);
		}
	}
}