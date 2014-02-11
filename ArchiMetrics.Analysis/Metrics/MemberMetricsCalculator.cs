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
	using ArchiMetrics.Common.Metrics;
	using Roslyn.Compilers.Common;
	using Roslyn.Compilers.CSharp;
	using Roslyn.Services;

	internal sealed class MemberMetricsCalculator : SemanticModelMetricsCalculator
	{
		private readonly ISolution _solution;
		private readonly CyclomaticComplexityCounter _counter = new CyclomaticComplexityCounter();
		private readonly LinesOfCodeCalculator _locCalculator = new LinesOfCodeCalculator();

		public MemberMetricsCalculator(ISemanticModel semanticModel, ISolution solution)
			: base(semanticModel)
		{
			_solution = solution;
		}

		public IEnumerable<IMemberMetric> Calculate(TypeDeclarationSyntaxInfo typeNode)
		{
			var walker = new MemberCollector(Root);
			var members = walker.GetMembers(Model, typeNode).ToArray();
			if ((typeNode.Syntax is ClassDeclarationSyntax
				|| typeNode.Syntax is StructDeclarationSyntax)
				&& members.All(m => m.Kind != SyntaxKind.ConstructorDeclaration))
			{
				var defaultConstructor = Syntax.ConstructorDeclaration(typeNode.Name)
											   .WithModifiers(Syntax.TokenList(Syntax.Token(SyntaxKind.PublicKeyword)))
											   .WithBody(Syntax.Block());
				members = members.Concat(new[] { defaultConstructor }).ToArray();
			}

			return CalculateMemberMetrics(members).ToArray();
		}

		public IMemberMetric Calculate(MethodDeclarationSyntax methodDeclaration)
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

		private IEnumerable<IMemberMetric> CalculateMemberMetrics(IEnumerable<SyntaxNode> nodes)
		{
			return from node in nodes
				   let metric = CalculateMemberMetric(node)
				   where metric != null
				   select metric;
		}

		private IMemberMetric CalculateMemberMetric(SyntaxNode syntaxNode)
		{
			var analyzer = new HalsteadAnalyzer();
			var halsteadMetrics = analyzer.Calculate(syntaxNode);

			var memberMetricKind = GetMemberMetricKind(syntaxNode.Kind);
			var source = CalculateClassCoupling(syntaxNode);
			var complexity = CalculateCyclomaticComplexity(syntaxNode);
			var linesOfCode = CalculateLinesOfCode(syntaxNode);
			var numberOfParameters = CalculateNumberOfParameters(syntaxNode);
			var numberOfLocalVariables = CalculateNumberOfLocalVariables(syntaxNode);
			var maintainabilityIndex = CalculateMaintainablityIndex(complexity, linesOfCode, halsteadMetrics);
			var afferentCouling = CalculateAfferentCouling(syntaxNode);
			var location = syntaxNode.GetLocation();
			var lineNumber = location.GetLineSpan(true).StartLinePosition.Line;
			var filePath = location.SourceTree == null ? string.Empty : location.SourceTree.FilePath;
			return new MemberMetric(
				filePath,
				halsteadMetrics,
				memberMetricKind,
				lineNumber,
				linesOfCode,
				maintainabilityIndex,
				complexity,
				syntaxNode.ToFullString(),
				source.ToArray(),
				numberOfParameters,
				numberOfLocalVariables,
				afferentCouling);
		}

		private int? CalculateAfferentCouling(SyntaxNode node)
		{
			try
			{
				return _solution == null
						   ? (int?)null
						   : Model.GetDeclaredSymbol(node)
								 .FindReferences(_solution)
								 .SelectMany(x => x.Locations)
								 .Count();
			}
			catch
			{
				// Some constructors are not present in syntax tree because they have been created for metrics calculation.
				return null;
			}
		}

		private int CalculateNumberOfLocalVariables(CommonSyntaxNode node)
		{
			var analyzer = new MethodLocalVariablesAnalyzer();
			return analyzer.Calculate(node);
		}

		private int CalculateNumberOfParameters(CommonSyntaxNode node)
		{
			var analyzer = new MethodParameterAnalyzer();
			return analyzer.Calculate(node);
		}
	}
}
