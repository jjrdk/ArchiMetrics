// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MemberMetricsCalculator.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the MemberMetricsCalculator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Metrics
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using ArchiMetrics.Common.Metrics;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;
	using Microsoft.CodeAnalysis.FindSymbols;

	internal sealed class MemberMetricsCalculator : SemanticModelMetricsCalculator
	{
		private readonly CyclomaticComplexityCounter _counter = new CyclomaticComplexityCounter();
		private readonly LinesOfCodeCalculator _locCalculator = new LinesOfCodeCalculator();
		private readonly Solution _solution;
		private readonly MemberNameResolver _nameResolver;

		public MemberMetricsCalculator(SemanticModel semanticModel, Solution solution)
			: base(semanticModel)
		{
			_solution = solution;
			_nameResolver = new MemberNameResolver(Model);
		}

		public async Task<IEnumerable<IMemberMetric>> Calculate(TypeDeclarationSyntaxInfo typeNode)
		{
			var walker = new MemberCollector();
			var members = walker.GetMembers(typeNode).ToArray();
			if ((typeNode.Syntax is ClassDeclarationSyntax
				|| typeNode.Syntax is StructDeclarationSyntax)
				&& members.All(m => m.CSharpKind() != SyntaxKind.ConstructorDeclaration))
			{
				var defaultConstructor = SyntaxFactory.ConstructorDeclaration(typeNode.Name)
											   .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
											   .WithBody(SyntaxFactory.Block());
				members = members.Concat(new[] { defaultConstructor }).ToArray();
			}

			var metrics = await CalculateMemberMetrics(members);
			return metrics.ToArray();
		}

		public Task<IMemberMetric> Calculate(MethodDeclarationSyntax methodDeclaration)
		{
			return CalculateMemberMetric(methodDeclaration);
		}

		private static double CalculateMaintainablityIndex(double cyclomaticComplexity, double linesOfCode, IHalsteadMetrics halsteadMetrics)
		{
			if (linesOfCode.Equals(0.0) || halsteadMetrics.NumberOfOperands.Equals(0) || halsteadMetrics.NumberOfOperators.Equals(0))
			{
				return 100.0;
			}

			var num = Math.Log(halsteadMetrics.GetVolume());
			var mi = ((171 - (5.2 * num) - (0.23 * cyclomaticComplexity) - (16.2 * Math.Log(linesOfCode))) * 100) / 171;

			return Math.Max(0.0, mi);
		}

		private static MemberMetricKind GetMemberMetricKind(SyntaxKind memberKind)
		{
			switch (memberKind)
			{
				case SyntaxKind.MethodDeclaration:
				case SyntaxKind.ConstructorDeclaration:
				case SyntaxKind.DestructorDeclaration:
					return MemberMetricKind.Method;

				case SyntaxKind.GetAccessorDeclaration:
				case SyntaxKind.SetAccessorDeclaration:
					return MemberMetricKind.PropertyAccessor;

				case SyntaxKind.AddAccessorDeclaration:
				case SyntaxKind.RemoveAccessorDeclaration:
					return MemberMetricKind.EventAccessor;
			}

			return MemberMetricKind.Unknown;
		}

		private int CalculateLinesOfCode(SyntaxNode node)
		{
			return _locCalculator.Calculate(node);
		}

		private int CalculateCyclomaticComplexity(SyntaxNode node)
		{
			return _counter.Calculate(node, Model);
		}

		private IEnumerable<ITypeCoupling> CalculateClassCoupling(SyntaxNode node)
		{
			var provider = new MemberClassCouplingAnalyzer(Model);
			return provider.Calculate(node);
		}

		private async Task<IEnumerable<IMemberMetric>> CalculateMemberMetrics(IEnumerable<SyntaxNode> nodes)
		{
			var tasks = nodes.Select(CalculateMemberMetric);

			var metrics = await Task.WhenAll(tasks);
			return from metric in metrics
				   where metric != null
				   select metric;
		}

		private async Task<IMemberMetric> CalculateMemberMetric(SyntaxNode syntaxNode)
		{
			var analyzer = new HalsteadAnalyzer();
			var halsteadMetrics = analyzer.Calculate(syntaxNode);
			var memberName = _nameResolver.TryResolveMemberSignatureString(syntaxNode);
			var memberMetricKind = GetMemberMetricKind(syntaxNode.CSharpKind());
			var source = CalculateClassCoupling(syntaxNode);
			var complexity = CalculateCyclomaticComplexity(syntaxNode);
			var linesOfCode = CalculateLinesOfCode(syntaxNode);
			var numberOfParameters = CalculateNumberOfParameters(syntaxNode);
			var numberOfLocalVariables = CalculateNumberOfLocalVariables(syntaxNode);
			var maintainabilityIndex = CalculateMaintainablityIndex(complexity, linesOfCode, halsteadMetrics);
			var afferentCoupling = await CalculateAfferentCoupling(syntaxNode);
			var location = syntaxNode.GetLocation();
			var lineNumber = location.GetLineSpan().StartLinePosition.Line;
			var filePath = location.SourceTree == null ? string.Empty : location.SourceTree.FilePath;
			return new MemberMetric(
				filePath, 
				halsteadMetrics, 
				memberMetricKind, 
				lineNumber, 
				linesOfCode, 
				maintainabilityIndex, 
				complexity, 
				memberName, 
				source.ToArray(), 
				numberOfParameters, 
				numberOfLocalVariables, 
				afferentCoupling);
		}

		private async Task<int> CalculateAfferentCoupling(SyntaxNode node)
		{
			try
			{
				if (_solution == null)
				{
					return 0;
				}

				var symbol = Model.GetDeclaredSymbol(node);
				var referenceTasks = SymbolFinder.FindReferencesAsync(symbol, _solution)
					.ContinueWith(t => t.Exception != null ? 0 : t.Result.Sum(x => x.Locations.Count()));

				return await referenceTasks;
			}
			catch
			{
				// Some constructors are not present in syntax tree because they have been created for metrics calculation.
				return 0;
			}
		}

		private int CalculateNumberOfLocalVariables(SyntaxNode node)
		{
			var analyzer = new MethodLocalVariablesAnalyzer();
			return analyzer.Calculate(node);
		}

		private int CalculateNumberOfParameters(SyntaxNode node)
		{
			var analyzer = new MethodParameterAnalyzer();
			return analyzer.Calculate(node);
		}
	}
}
