// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CodeMetricsCalculator.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the CodeMetricsCalculator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text.RegularExpressions;
	using System.Threading.Tasks;
	using ArchiMetrics.Analysis.Metrics;
	using ArchiMetrics.Common;
	using ArchiMetrics.Common.Metrics;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;

	public class CodeMetricsCalculator : ICodeMetricsCalculator
	{
		private readonly IAsyncFactory<ISymbol, ITypeDocumentation> _typeDocumentationFactory;
		private readonly IAsyncFactory<ISymbol, IMemberDocumentation> _memberDocumentationFactory;
		private static readonly List<Regex> Patterns = new List<Regex>
													   {
														   new Regex(@".*\.g\.cs$", RegexOptions.Compiled), 
														   new Regex(@".*\.g\.i\.cs$", RegexOptions.Compiled), 
														   new Regex(@".*\.designer\.cs$", RegexOptions.Compiled)
													   };

		private readonly SyntaxCollector _syntaxCollector = new SyntaxCollector();

		public CodeMetricsCalculator()
			: this(new TypeDocumentationFactory(), new MemberDocumentationFactory())
		{
		}

		public CodeMetricsCalculator(IAsyncFactory<ISymbol, ITypeDocumentation> typeDocumentationFactory, IAsyncFactory<ISymbol, IMemberDocumentation> memberDocumentationFactory)
		{
			_typeDocumentationFactory = typeDocumentationFactory;
			_memberDocumentationFactory = memberDocumentationFactory;
		}

		public virtual async Task<IEnumerable<INamespaceMetric>> Calculate(Project project, Solution solution)
		{
			var compilation = await project.GetCompilationAsync().ConfigureAwait(false);
			var namespaceDeclarations = await GetNamespaceDeclarations(project).ConfigureAwait(false);
			return await CalculateNamespaceMetrics(namespaceDeclarations, compilation, solution).ConfigureAwait(false);
		}

		public async Task<IEnumerable<INamespaceMetric>> Calculate(IEnumerable<SyntaxTree> syntaxTrees)
		{
			var trees = syntaxTrees.AsArray();
			var commonCompilation = CSharpCompilation.Create("x", syntaxTrees: trees);
			var declarations = _syntaxCollector.GetDeclarations(trees);
			var statementMembers = declarations.Statements.Select(s =>
				s is StatementSyntax
				? SyntaxFactory.MethodDeclaration(
					SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword)),
					Guid.NewGuid().ToString("N"))
					.WithBody(SyntaxFactory.Block(s as StatementSyntax))
					: s);
			var members = declarations.MemberDeclarations.Concat(statementMembers).AsArray();
			var anonClass = members.Any()
								? new[]
								  {
									  SyntaxFactory.ClassDeclaration(
										  "UnnamedClass")
										  .WithModifiers(
											  SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
										  .WithMembers(SyntaxFactory.List(members))
								  }
								: new TypeDeclarationSyntax[0];
			var array = declarations.TypeDeclarations
				.Concat(anonClass)
				.Cast<MemberDeclarationSyntax>()
				.AsArray();
			var anonNs = array.Any()
				? new[]
						  {
							  SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName("Unnamed"))
								  .WithMembers(SyntaxFactory.List(array))
						  }
				: new NamespaceDeclarationSyntax[0];
			var namespaceDeclarations = declarations
				.NamespaceDeclarations
				.Concat(anonNs)
				.Select(x => new NamespaceDeclarationSyntaxInfo
				{
					Name = x.GetName(x),
					Syntax = x
				})
				.GroupBy(x => x.Name)
				.Select(g => new NamespaceDeclaration
				{
					Name = g.Key,
					SyntaxNodes = g.AsArray()
				})
				.AsArray();

			var namespaceMetrics = await CalculateNamespaceMetrics(namespaceDeclarations, commonCompilation, null).ConfigureAwait(false);
			return namespaceMetrics;
		}

		private static async Task<INamespaceMetric> CalculateNamespaceMetrics(Compilation compilation, NamespaceDeclaration namespaceNodes, IEnumerable<ITypeMetric> typeMetrics)
		{
			var namespaceNode = namespaceNodes.SyntaxNodes.FirstOrDefault();
			if (namespaceNode == null)
			{
				return null;
			}

			var tuple = await VerifyCompilation(compilation, namespaceNode).ConfigureAwait(false);
			compilation = tuple.Item1;
			var semanticModel = compilation.GetSemanticModel(tuple.Item3);
			var calculator = new NamespaceMetricsCalculator(semanticModel);
			return calculator.CalculateFrom(namespaceNode, typeMetrics);
		}

		private async Task<Tuple<Compilation, ITypeMetric>> CalculateTypeMetrics(Solution solution, Compilation compilation, TypeDeclaration typeNodes, IEnumerable<IMemberMetric> memberMetrics)
		{
			if (typeNodes.SyntaxNodes.Any())
			{
				var tuple = await VerifyCompilation(compilation, typeNodes.SyntaxNodes.First()).ConfigureAwait(false);
				var semanticModel = tuple.Item2;
				compilation = tuple.Item1;
				var typeNode = tuple.Item3;
				var calculator = new TypeMetricsCalculator(semanticModel, solution, _typeDocumentationFactory);
				var metrics = await calculator.CalculateFrom(typeNode, memberMetrics);
				return new Tuple<Compilation, ITypeMetric>(
					compilation,
					metrics);
			}

			return null;
		}

		private static async Task<Tuple<Compilation, SemanticModel, TypeDeclarationSyntaxInfo>> VerifyCompilation(Compilation compilation, TypeDeclarationSyntaxInfo typeNode)
		{
			var tree = typeNode.Syntax.SyntaxTree;

			if (tree == null)
			{
				var cu = CSharpSyntaxTree.Create(
					SyntaxFactory
					.CompilationUnit()
					.WithMembers(SyntaxFactory.List(new[] { (MemberDeclarationSyntax)typeNode.Syntax })));
				var root = await cu.GetRootAsync().ConfigureAwait(false);
				typeNode.Syntax = (TypeDeclarationSyntax)root.ChildNodes().First();
				var newCompilation = compilation.AddSyntaxTrees(cu);
				var semanticModel = newCompilation.GetSemanticModel(cu);
				return new Tuple<Compilation, SemanticModel, TypeDeclarationSyntaxInfo>(newCompilation, semanticModel, typeNode);
			}

			var result = AddToCompilation(compilation, tree);
			var childNodes = result.Item2.GetRoot().DescendantNodesAndSelf();
			typeNode.Syntax = childNodes.OfType<TypeDeclarationSyntax>().First();
			return new Tuple<Compilation, SemanticModel, TypeDeclarationSyntaxInfo>(
				result.Item1,
				result.Item1.GetSemanticModel(result.Item2),
				typeNode);
		}

		private static Tuple<Compilation, SyntaxTree> AddToCompilation(Compilation compilation, SyntaxTree tree)
		{
			if (!compilation.ContainsSyntaxTree(tree))
			{
				var newTree = tree;
				if (!tree.HasCompilationUnitRoot)
				{
					var childNodes = tree.GetRoot()
						.ChildNodes()
						.AsArray();
					newTree = CSharpSyntaxTree.Create(SyntaxFactory.CompilationUnit()
						.WithMembers(
							SyntaxFactory.List(childNodes.OfType<MemberDeclarationSyntax>()))
						.WithUsings(
							SyntaxFactory.List(childNodes.OfType<UsingDirectiveSyntax>()))
						.WithExterns(
							SyntaxFactory.List(childNodes.OfType<ExternAliasDirectiveSyntax>())));
				}

				var comp = compilation.AddSyntaxTrees(newTree);
				return new Tuple<Compilation, SyntaxTree>(comp, newTree);
			}

			return new Tuple<Compilation, SyntaxTree>(compilation, tree);
		}

		private static async Task<Tuple<Compilation, SemanticModel, SyntaxTree, NamespaceDeclarationSyntaxInfo>> VerifyCompilation(Compilation compilation, NamespaceDeclarationSyntaxInfo namespaceNode)
		{
			SemanticModel semanticModel;
			var tree = namespaceNode.Syntax.SyntaxTree;
			if (tree == null)
			{
				var compilationUnit = SyntaxFactory.CompilationUnit()
					.WithMembers(SyntaxFactory.List(new[] { (MemberDeclarationSyntax)namespaceNode.Syntax }));
				var cu = CSharpSyntaxTree.Create(compilationUnit);
				var root = await cu.GetRootAsync().ConfigureAwait(false);
				namespaceNode.Syntax = root.ChildNodes().First();
				var newCompilation = compilation.AddSyntaxTrees(cu);
				semanticModel = newCompilation.GetSemanticModel(cu);
				return new Tuple<Compilation, SemanticModel, SyntaxTree, NamespaceDeclarationSyntaxInfo>(newCompilation, semanticModel, cu, namespaceNode);
			}

			var result = AddToCompilation(compilation, tree);
			compilation = result.Item1;
			tree = result.Item2;
			semanticModel = compilation.GetSemanticModel(tree);
			return new Tuple<Compilation, SemanticModel, SyntaxTree, NamespaceDeclarationSyntaxInfo>(compilation, semanticModel, tree, namespaceNode);
		}

		private static async Task<IEnumerable<NamespaceDeclaration>> GetNamespaceDeclarations(Project project)
		{
			var namespaceDeclarationTasks = project.Documents
				.Select(document => new { document, codeFile = document.FilePath })
				.Where(t => !IsGeneratedCodeFile(t.document, Patterns))
				.Select(
					async t =>
					{
						var collector = new NamespaceCollector();
						var root = await t.document.GetSyntaxRootAsync().ConfigureAwait(false);
						return new
							   {
								   t.codeFile,
								   namespaces = collector.GetNamespaces(root)
							   };
					})
				.Select(
					async t =>
					{
						var result = await t.ConfigureAwait(false);
						return result.namespaces
							.Select(
								x => new NamespaceDeclarationSyntaxInfo
									 {
										 Name = x.GetName(x.SyntaxTree.GetRoot()),
										 CodeFile = result.codeFile,
										 Syntax = x
									 });
					});
			var namespaceDeclarations = await Task.WhenAll(namespaceDeclarationTasks).ConfigureAwait(false);
			return namespaceDeclarations
				.SelectMany(x => x)
				.GroupBy(x => x.Name)
				.Select(y => new NamespaceDeclaration { Name = y.Key, SyntaxNodes = y });
		}

		private static bool IsGeneratedCodeFile(Document doc, IEnumerable<Regex> patterns)
		{
			var path = doc.FilePath;
			return !string.IsNullOrWhiteSpace(path) && patterns.Any(x => x.IsMatch(path));
		}

		private static IEnumerable<TypeDeclaration> GetTypeDeclarations(NamespaceDeclaration namespaceDeclaration)
		{
			var collector = new TypeCollector();
			return namespaceDeclaration.SyntaxNodes
				.Select(info =>
				{
					Func<TypeDeclarationSyntax, TypeDeclarationSyntaxInfo> selector =
						x => new TypeDeclarationSyntaxInfo(info.CodeFile, x.SyntaxTree == null ? x.Identifier.ValueText : x.GetName(x.SyntaxTree.GetRoot()), x);
					return new { info, selector };
				})
				.SelectMany(x => collector.GetTypes(x.info.Syntax).Select(x.selector))
				.GroupBy(x => x.Name)
				.Select(x => new TypeDeclaration { Name = x.Key, SyntaxNodes = x });
		}

		private async Task<IEnumerable<INamespaceMetric>> CalculateNamespaceMetrics(IEnumerable<NamespaceDeclaration> namespaceDeclarations, Compilation compilation, Solution solution)
		{
			var tasks = namespaceDeclarations.Select(
				async arg =>
				{
					var tuple = await CalculateTypeMetrics(compilation, arg, solution).ConfigureAwait(false);
					return CalculateNamespaceMetrics(tuple.Item1, arg, tuple.Item2.AsArray());
				})
					.AsArray();
			var x = await Task.WhenAll(tasks).ConfigureAwait(false);
			return await Task.WhenAll(x).ConfigureAwait(false);
		}

		private async Task<Tuple<Compilation, IEnumerable<IMemberMetric>>> CalculateMemberMetrics(Compilation compilation, TypeDeclaration typeNodes, Solution solution)
		{
			var comp = compilation;
			var metrics = typeNodes.SyntaxNodes
				.Select(async info =>
				{
					var tuple = await VerifyCompilation(comp, info).ConfigureAwait(false);
					var semanticModel = tuple.Item2;
					comp = tuple.Item1;
					var calculator = new MemberMetricsCalculator(semanticModel, solution, _memberDocumentationFactory);

					return await calculator.Calculate(info).ConfigureAwait(false);
				});
			var results = await Task.WhenAll(metrics).ConfigureAwait(false);
			return new Tuple<Compilation, IEnumerable<IMemberMetric>>(comp, results.SelectMany(x => x).AsArray());
		}

		private async Task<Tuple<Compilation, IEnumerable<ITypeMetric>>> CalculateTypeMetrics(Compilation compilation, NamespaceDeclaration namespaceNodes, Solution solution)
		{
			var comp = compilation;
			var tasks = GetTypeDeclarations(namespaceNodes)
				.Select(async typeNodes =>
					{
						var tuple = await CalculateMemberMetrics(comp, typeNodes, solution).ConfigureAwait(false);
						var metrics = tuple.Item2;
						comp = tuple.Item1;
						return new
						{
							comp,
							typeNodes,
							solution,
							memberMetrics = metrics
						};
					})
					.AsArray();
			var data = await Task.WhenAll(tasks).ConfigureAwait(false);
			var typeMetricsTasks = data
				.Select(async item =>
				{
					var tuple = await CalculateTypeMetrics(item.solution, item.comp, item.typeNodes, item.memberMetrics).ConfigureAwait(false);
					if (tuple == null)
					{
						return null;
					}

					comp = tuple.Item1;
					return tuple.Item2;
				})
				.AsArray();

			var typeMetrics = await Task.WhenAll(typeMetricsTasks).ConfigureAwait(false);
			var array = typeMetrics.Where(x => x != null).AsArray();
			return new Tuple<Compilation, IEnumerable<ITypeMetric>>(comp, array);
		}
	}
}
