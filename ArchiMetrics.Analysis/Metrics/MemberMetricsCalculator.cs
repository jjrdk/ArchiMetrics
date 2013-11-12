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

	internal sealed class MemberMetricsCalculator : SemanticModelMetricsCalculator
	{
		private readonly CyclomaticComplexityCounter _counter = new CyclomaticComplexityCounter();

		public MemberMetricsCalculator(ISemanticModel semanticModel)
			: base(semanticModel)
		{
		}

		public IEnumerable<IMemberMetric> Calculate(TypeDeclarationSyntaxInfo typeNode)
		{
			var walker = new MemberCollector(Root);
			var members = walker.GetMembers(Model, typeNode).ToArray();
			if ((typeNode.Syntax is ClassDeclarationSyntax
				|| typeNode.Syntax is StructDeclarationSyntax)
				&& members.All(m => m.Kind != MemberKind.Constructor))
			{
				var defaultConstructor = Syntax.ConstructorDeclaration(typeNode.Name)
											   .WithModifiers(Syntax.TokenList(Syntax.Token(SyntaxKind.PublicKeyword)))
											   .WithBody(Syntax.Block());
				members = members.Concat(new[]
										 {
											 new MemberNode(typeNode.CodeFile, typeNode.Name, MemberKind.Constructor, 0, defaultConstructor, Model)
										 })
										 .ToArray();
			}

			return CalculateMemberMetrics(members).ToArray();
		}

		public IMemberMetric Calculate(MethodDeclarationSyntax methodDeclaration)
		{
			var member = new MemberNode(string.Empty, methodDeclaration.Identifier.ValueText, MemberKind.Method, 0, methodDeclaration, Model);
			return CalculateMemberMetric(member);
		}

		private static int CalculateLinesOfCode(MemberNode node)
		{
			var provider = new StatementsAnalyzer();
			return provider.Calculate(node);
		}

		private static int CalculateLogicalComplexity(MemberNode node)
		{
			var provider = new LogicalComplexityCounter();
			return provider.Calculate(node);
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

		private static MemberMetricKind GetMemberMetricKind(MemberNode memberNode)
		{
			switch (memberNode.Kind)
			{
				case MemberKind.Method:
				case MemberKind.Constructor:
				case MemberKind.Destructor:
					return MemberMetricKind.Method;

				case MemberKind.GetProperty:
				case MemberKind.SetProperty:
					return MemberMetricKind.PropertyAccessor;

				case MemberKind.AddEventHandler:
				case MemberKind.RemoveEventHandler:
					return MemberMetricKind.EventAccessor;
			}

			return MemberMetricKind.Unknown;
		}

		private int CalculateCyclomaticComplexity(MemberNode node)
		{
			return _counter.Calculate(node);
		}

		private IEnumerable<TypeCoupling> CalculateClassCoupling(MemberNode node)
		{
			var provider = new MemberClassCouplingAnalyzer(Model);
			return provider.Calculate(node);
		}

		private IEnumerable<IMemberMetric> CalculateMemberMetrics(IEnumerable<MemberNode> nodes)
		{
			return from node in nodes
				   let metric = CalculateMemberMetric(node)
				   where metric != null
				   select metric;
		}

		private IMemberMetric CalculateMemberMetric(MemberNode node)
		{
			var analyzer = new HalsteadAnalyzer();
			var halsteadMetrics = analyzer.Calculate(node);
			if (halsteadMetrics == null)
			{
				return null;
			}

			var syntaxNode = node.SyntaxNode;
			var memberMetricKind = GetMemberMetricKind(node);
			var source = CalculateClassCoupling(node);
			var complexity = CalculateCyclomaticComplexity(node);
			var logicalComplexity = CalculateLogicalComplexity(node);
			var linesOfCode = CalculateLinesOfCode(node);
			var numberOfParameters = CalculateNumberOfParameters(syntaxNode);
			var numberOfLocalVariables = CalculateNumberOfLocalVariables(syntaxNode);
			var maintainabilityIndex = CalculateMaintainablityIndex(complexity, linesOfCode, halsteadMetrics);
			return new MemberMetric(
				node.CodeFile, 
				halsteadMetrics, 
				memberMetricKind, 
				node.LineNumber, 
				linesOfCode, 
				maintainabilityIndex, 
				complexity, 
				node.DisplayName, 
				logicalComplexity, 
				source.ToArray(), 
				numberOfParameters, 
				numberOfLocalVariables);
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
