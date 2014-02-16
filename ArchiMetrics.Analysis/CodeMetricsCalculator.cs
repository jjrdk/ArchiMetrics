// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CodeMetricsCalculator.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
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
	using ArchiMetrics.Common.Metrics;
	using Roslyn.Compilers.Common;
	using Roslyn.Compilers.CSharp;
	using Roslyn.Services;

	public class CodeMetricsCalculator : ICodeMetricsCalculator
	{
		private static readonly List<Regex> Patterns = new List<Regex>
													   {
														   new Regex(@".*\.g\.cs$", RegexOptions.Compiled), 
														   new Regex(@".*\.g\.i\.cs$", RegexOptions.Compiled), 
														   new Regex(@".*\.designer\.cs$", RegexOptions.Compiled)
													   };

		private readonly SyntaxCollector _syntaxCollector = new SyntaxCollector();

		public virtual async Task<IEnumerable<INamespaceMetric>> Calculate(IProject project, ISolution solution)
		{
			var calcProject = project.WithDocuments();
			var compilation = await calcProject.GetCompilationAsync();
			var namespaceDeclarations = await GetNamespaceDeclarations(calcProject);
			return await CalculateNamespaceMetrics(namespaceDeclarations, compilation, solution);
		}

		public async Task<IEnumerable<INamespaceMetric>> Calculate(IEnumerable<SyntaxTree> syntaxTrees)
		{
			var trees = syntaxTrees.ToArray();
			var commonCompilation = Compilation.Create("x", syntaxTrees: trees);
			var declarations = _syntaxCollector.GetDeclarations(trees);
			var statementMembers = declarations.Statements.Select(s =>
				Syntax.MethodDeclaration(
					Syntax.PredefinedType(Syntax.Token(SyntaxKind.VoidKeyword)),
					Guid.NewGuid().ToString("N"))
					.WithBody(Syntax.Block(s)));
			var members = declarations.MemberDeclarations.Concat(statementMembers).ToArray();
			var anonClass = members.Any()
				? new[]
						  {
							  Syntax.ClassDeclaration(
								  "UnnamedClass")
								  .WithModifiers(Syntax.Token(SyntaxKind.PublicKeyword))
								  .WithMembers(Syntax.List(members))
						  }
				: new TypeDeclarationSyntax[0];
			var array = declarations.TypeDeclarations
				.Concat(anonClass)
				.Cast<MemberDeclarationSyntax>()
				.ToArray();
			var anonNs = array.Any()
				? new[]
						  {
							  Syntax.NamespaceDeclaration(Syntax.ParseName("Unnamed"))
								  .WithMembers(Syntax.List(array))
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
					SyntaxNodes = g.ToArray()
				})
				.ToArray();

			var namespaceMetrics = await CalculateNamespaceMetrics(namespaceDeclarations, commonCompilation, null);
			return namespaceMetrics;
		}

		private static async Task<INamespaceMetric> CalculateNamespaceMetrics(CommonCompilation compilation, NamespaceDeclaration namespaceNodes, IEnumerable<ITypeMetric> typeMetrics)
		{
			var namespaceNode = namespaceNodes.SyntaxNodes.FirstOrDefault();
			if (namespaceNode == null)
			{
				return null;
			}

			var tuple = await VerifyCompilation(compilation, namespaceNode);
			compilation = tuple.Item1;
			var semanticModel = compilation.GetSemanticModel(namespaceNode.Syntax.SyntaxTree);
			var calculator = new NamespaceMetricsCalculator(semanticModel);
			return calculator.CalculateFrom(namespaceNode, typeMetrics);
		}

		private static async Task<Tuple<CommonCompilation, ITypeMetric>> CalculateTypeMetrics(CommonCompilation compilation, TypeDeclaration typeNodes, IEnumerable<IMemberMetric> memberMetrics)
		{
			if (typeNodes.SyntaxNodes.Any())
			{
				var tuple = await VerifyCompilation(compilation, typeNodes.SyntaxNodes.First());
				var semanticModel = tuple.Item2;
				compilation = tuple.Item1;
				var typeNode = tuple.Item3;
				var calculator = new TypeMetricsCalculator(semanticModel);
				return new Tuple<CommonCompilation, ITypeMetric>(
					compilation,
					calculator.CalculateFrom(typeNode, memberMetrics));
			}

			return null;
		}

		private static async Task<Tuple<CommonCompilation, ISemanticModel, TypeDeclarationSyntaxInfo>> VerifyCompilation(CommonCompilation compilation, TypeDeclarationSyntaxInfo typeNode)
		{
			ISemanticModel semanticModel;
			if (typeNode.Syntax.SyntaxTree == null)
			{
				var cu = SyntaxTree.Create(
					Syntax
					.CompilationUnit()
					.WithMembers(Syntax.List((MemberDeclarationSyntax)typeNode.Syntax)));
				var root = await cu.GetRootAsync();
				typeNode.Syntax = root.ChildNodes().First();
				var newCompilation = compilation.AddSyntaxTrees(cu);
				semanticModel = newCompilation.GetSemanticModel(cu);
				return new Tuple<CommonCompilation, ISemanticModel, TypeDeclarationSyntaxInfo>(newCompilation, semanticModel, typeNode);
			}

			if (!compilation.ContainsSyntaxTree(typeNode.Syntax.SyntaxTree))
			{
				compilation = compilation.AddSyntaxTrees(typeNode.Syntax.SyntaxTree);
			}

			semanticModel = compilation.GetSemanticModel(typeNode.Syntax.SyntaxTree);
			return new Tuple<CommonCompilation, ISemanticModel, TypeDeclarationSyntaxInfo>(
				compilation,
				semanticModel,
				typeNode);
		}

		private static async Task<Tuple<CommonCompilation, ISemanticModel, NamespaceDeclarationSyntaxInfo>> VerifyCompilation(CommonCompilation compilation, NamespaceDeclarationSyntaxInfo namespaceNode)
		{
			ISemanticModel semanticModel;
			if (namespaceNode.Syntax.SyntaxTree == null)
			{
				var compilationUnit = Syntax.CompilationUnit()
					.WithMembers(Syntax.List((MemberDeclarationSyntax)namespaceNode.Syntax));
				var cu = SyntaxTree.Create(compilationUnit);
				var root = await cu.GetRootAsync();
				namespaceNode.Syntax = root.ChildNodes().First();
				var newCompilation = compilation.AddSyntaxTrees(cu);
				semanticModel = newCompilation.GetSemanticModel(cu);
				return new Tuple<CommonCompilation, ISemanticModel, NamespaceDeclarationSyntaxInfo>(newCompilation, semanticModel, namespaceNode);
			}

			if (!compilation.ContainsSyntaxTree(namespaceNode.Syntax.SyntaxTree))
			{
				compilation = compilation.AddSyntaxTrees(namespaceNode.Syntax.SyntaxTree);
			}

			semanticModel = compilation.GetSemanticModel(namespaceNode.Syntax.SyntaxTree);
			return new Tuple<CommonCompilation, ISemanticModel, NamespaceDeclarationSyntaxInfo>(compilation, semanticModel, namespaceNode);
		}

		private static async Task<IEnumerable<NamespaceDeclaration>> GetNamespaceDeclarations(IProject project)
		{
			var namespaceDeclarationTasks = project.Documents
				.Select(document => new { document, codeFile = document.FilePath })
				.Where(t => !IsGeneratedCodeFile(t.document, Patterns))
				.Select(
					async t =>
					{
						var collector = new NamespaceCollector();
						var root = await t.document.GetSyntaxRootAsync();
						return new
							   {
								   t.codeFile,
								   namespaces = collector.GetNamespaces(root)
							   };
					})
				.Select(
					async t =>
					{
						var result = await t;
						return result.namespaces
							.Select(
								x => new NamespaceDeclarationSyntaxInfo
									 {
										 Name = x.GetName(x.SyntaxTree.GetRoot()),
										 CodeFile = result.codeFile,
										 Syntax = x
									 });
					});
			var namespaceDeclarations = await Task.WhenAll(namespaceDeclarationTasks);
			return namespaceDeclarations
				.SelectMany(x => x)
				.GroupBy(x => x.Name)
				.Select(y => new NamespaceDeclaration { Name = y.Key, SyntaxNodes = y });
		}

		private static bool IsGeneratedCodeFile(IDocument doc, IEnumerable<Regex> patterns)
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

		private async Task<IEnumerable<INamespaceMetric>> CalculateNamespaceMetrics(IEnumerable<NamespaceDeclaration> namespaceDeclarations, CommonCompilation compilation, ISolution solution)
		{
			var tasks = namespaceDeclarations.Select(
				arg => CalculateTypeMetrics(compilation, arg, solution)
								 .ContinueWith(t => CalculateNamespaceMetrics(t.Result.Item1, arg, t.Result.Item2.ToArray())))
					.ToArray();
			var x = await Task.WhenAll(tasks);
			return await Task.WhenAll(x);
		}

		private async Task<Tuple<CommonCompilation, IEnumerable<IMemberMetric>>> CalculateMemberMetrics(CommonCompilation compilation, TypeDeclaration typeNodes, ISolution solution)
		{
			var comp = compilation;
			var metrics = typeNodes.SyntaxNodes
				.Select(async info =>
				{
					var tuple = await VerifyCompilation(comp, info);
					var semanticModel = tuple.Item2;
					comp = tuple.Item1;
					var calculator = new MemberMetricsCalculator(semanticModel, solution);

					return await calculator.Calculate(info);
				});
			var results = await Task.WhenAll(metrics);
			return new Tuple<CommonCompilation, IEnumerable<IMemberMetric>>(comp, results.SelectMany(x => x).ToArray());
		}

		private async Task<Tuple<CommonCompilation, IEnumerable<ITypeMetric>>> CalculateTypeMetrics(CommonCompilation compilation, NamespaceDeclaration namespaceNodes, ISolution solution)
		{
			var comp = compilation;
			var tasks = GetTypeDeclarations(namespaceNodes)
				.Select(async typeNodes =>
					{
						var tuple = await CalculateMemberMetrics(comp, typeNodes, solution);
						var metrics = tuple.Item2;
						comp = tuple.Item1;
						return new
						{
							comp,
							typeNodes,
							memberMetrics = metrics
						};
					})
					.ToArray();
			var data = await Task.WhenAll(tasks);
			var typeMetricsTasks = data
				.Select(async item =>
				{
					var tuple = await CalculateTypeMetrics(item.comp, item.typeNodes, item.memberMetrics);
					if (tuple == null)
					{
						return null;
					}

					comp = tuple.Item1;
					return tuple.Item2;
				})
				.ToArray();

			var typeMetrics = await Task.WhenAll(typeMetricsTasks);
			var array = typeMetrics.Where(x => x != null).ToArray();
			return new Tuple<CommonCompilation, IEnumerable<ITypeMetric>>(comp, array);
		}
	}
}
