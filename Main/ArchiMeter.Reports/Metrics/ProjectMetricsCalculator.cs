// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectMetricsCalculator.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ProjectMetricsCalculator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMeter.Reports.Metrics
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Text.RegularExpressions;
	using System.Xml.Linq;
	using Analysis.Xaml;
	using Core.Metrics;
	using Roslyn.Compilers.Common;
	using Roslyn.Compilers.CSharp;
	using Roslyn.Services;

	public sealed class ProjectMetricsCalculator : IProjectMetricsCalculator
	{
		private static readonly List<Regex> Patterns = new List<Regex>
			                                               {
				                                               new Regex(@".*\.g\.cs$", RegexOptions.Compiled),
				                                               new Regex(@".*\.g\.i\.cs$", RegexOptions.Compiled),
															   new Regex(@".*\.xaml\.cs$", RegexOptions.Compiled),
															   new Regex(@".*\.designer\.cs$", RegexOptions.Compiled)
			                                               };

		private readonly XamlConverter _converter;

		public ProjectMetricsCalculator()
		{
			_converter = new XamlConverter();
			IgnoreGeneratedCode = true;
		}

		// Properties
		public bool IgnoreGeneratedCode { get; set; }

		// Methods
		public IEnumerable<NamespaceMetric> Calculate(IProject project)
		{
			var calcProject = project.HasDocuments ? project : GetDocuments(project);
			var compilation = calcProject.GetCompilation();
			var namespaceDeclarations = GetNamespaceDeclarations(calcProject, IgnoreGeneratedCode);
			var metrics = namespaceDeclarations.Select(pair => pair.Value)
											   .Select(
												   arg =>
												   new
													   {
														   NamespaceDeclaration = arg,
														   Metrics = CalculateTypeMetrics(compilation, arg)
													   })
											   .Select(
												   b => CalculateNamespaceMetrics(compilation, b.NamespaceDeclaration, b.Metrics));
			return metrics;
		}

		private static IEnumerable<MemberMetric> CalculateMemberMetrics(CommonCompilation compilation, TypeDeclaration typeNodes)
		{
			return typeNodes.SyntaxNodes
							.SelectMany(info => new MemberMetricsCalculator(compilation.GetSemanticModel(info.Syntax.SyntaxTree))
													.Calculate(info));
		}

		private static NamespaceMetric CalculateNamespaceMetrics(CommonCompilation compilation, NamespaceDeclaration namespaceNodes, IEnumerable<TypeMetric> typeMetrics)
		{
			var namespaceNode = namespaceNodes.SyntaxNodes.FirstOrDefault();
			if (namespaceNode == null)
			{
				return null;
			}

			var calculator = new NamespaceMetricsCalculator(compilation.GetSemanticModel(namespaceNode.Syntax.SyntaxTree));
			return calculator.CalculateFrom(namespaceNode, typeMetrics);
		}

		private static IEnumerable<TypeMetric> CalculateTypeMetrics(CommonCompilation compilation, NamespaceDeclaration namespaceNodes)
		{
			return GetTypeDeclarations(namespaceNodes)
				.Select(pair => pair.Value)
				.Select(typeNodes => new { typeNodes, memberMetrics = CalculateMemberMetrics(compilation, typeNodes) })
				.Select(@t => CalculateTypeMetrics(compilation, @t.typeNodes, @t.memberMetrics));
		}

		private static TypeMetric CalculateTypeMetrics(CommonCompilation compilation, TypeDeclaration typeNodes, IEnumerable<MemberMetric> memberMetrics)
		{
			if (typeNodes.SyntaxNodes.Any())
			{
				var typeNode = typeNodes.SyntaxNodes.First();
				var calculator = new TypeMetricsCalculator(compilation.GetSemanticModel(typeNode.Syntax.SyntaxTree));
				return calculator.CalculateFrom(typeNode, memberMetrics);
			}

			return null;
		}

		private static IDictionary<string, NamespaceDeclaration> GetNamespaceDeclarations(IProject project, bool ignoreGeneratedCode = false)
		{
			return project.Documents
						  .Select(document => new { document, codeFile = document.FilePath })
						  .Where(@t => !ignoreGeneratedCode || !@t.document.IsGeneratedCodeFile(Patterns))
						  .Select(@t => new { @t, collector = new NamespaceCollectorSyntaxWalker() })
						  .Select(@t => new { @t, syntaxRoot = @t.@t.document.GetSyntaxRoot() })
						  .SelectMany(@t => @t.@t.collector.GetNamespaces<NamespaceDeclarationSyntax>(@t.syntaxRoot)
											   .Select(x => new NamespaceDeclarationSyntaxInfo
																{
																	Name = x.GetName(x.SyntaxTree.GetRoot()),
																	CodeFile = @t.@t.@t.codeFile,
																	Syntax = x
																}))
						  .GroupBy(x => x.Name)
						  .ToDictionary(
							  x => x.Key,
							  y => new NamespaceDeclaration
									   {
										   Name = y.Key,
										   SyntaxNodes = y
									   });
		}

		private static IDictionary<string, TypeDeclaration> GetTypeDeclarations(NamespaceDeclaration namespaceDeclaration)
		{
			return namespaceDeclaration.SyntaxNodes.Select(namespaceNode => new { namespaceNode, node = namespaceNode })
									   .Select(@t => new
														 {
															 @t,
															 selector = (Func<TypeDeclarationSyntax, TypeDeclarationSyntaxInfo>)
																		(x =>
																		 new TypeDeclarationSyntaxInfo(
																			 @t.node.CodeFile,
																			 x.GetName(x.SyntaxTree.GetRoot()),
																			 x))
														 })
									   .Select(@t => new { @t, collector = new TypeCollectorSyntaxWalker() })
									   .SelectMany(
										   @t =>
										   @t.collector.GetTypes<TypeDeclarationSyntax>(@t.@t.@t.namespaceNode.Syntax)
											 .Select<TypeDeclarationSyntax, TypeDeclarationSyntaxInfo>(@t.@t.selector))
									   .GroupBy(x => x.Name)
									   .ToDictionary(
										   x => x.Key,
										   y => new TypeDeclaration
													{
														Name = y.Key,
														SyntaxNodes = y
													});
		}

		private IProject GetDocuments(IProject project)
		{
			var doc = XDocument.Load(project.FilePath);
			var defaultNs = doc.Root.GetDefaultNamespace();
			var compiles = doc.Descendants(defaultNs + "Compile");
			var dependents = doc.Descendants(defaultNs + "DependentUpon");
			var filePaths =
				compiles
				   .Select(x => x.Attribute("Include").Value)
				   .Concat(dependents.Select(x => x.Value))
				   .OrderByDescending(x => x);

			project = filePaths.Aggregate(
				project,
				(p, s) =>
				{
					DocumentId did;
					var root = Path.GetDirectoryName(project.FilePath);
					var filepath = Path.Combine(root, s);
					var sourceCode = (s.EndsWith(".xaml", StringComparison.OrdinalIgnoreCase))
										 ? _converter.Convert(filepath)
										 : SyntaxTree.ParseFile(filepath);
					return p.AddDocument(s, sourceCode.GetText(), out did);
				});

			return project;
		}
	}
}