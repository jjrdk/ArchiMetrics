// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MemberMetricsCalculator.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the MemberMetricsCalculator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMeter.Analysis.Metrics
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Common.Metrics;
	using Roslyn.Compilers.Common;
	using Roslyn.Compilers.CSharp;

	internal sealed class MemberMetricsCalculator : SemanticModelMetricsCalculator
	{
		private readonly CyclomaticComplexityAnalyzer _analyzer = new CyclomaticComplexityAnalyzer();

		public MemberMetricsCalculator(ISemanticModel semanticModel)
			: base(semanticModel)
		{
		}

		public IEnumerable<MemberMetric> Calculate(TypeDeclarationSyntaxInfo typeNode)
		{
			var walker = new MemberCollectorSyntaxWalker(Root);
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
											 new MemberNode(typeNode.CodeFile, typeNode.Name, MemberKind.Constructor, 0, defaultConstructor)
										 })
										 .ToArray();
			}

			return CalculateMemberMetrics(members).ToArray();
		}

		private int CalculateCyclomaticComplexity(MemberNode node)
		{
			return _analyzer.Calculate(node);
		}

		private static int CalculateLinesOfCode(MemberNode node)
		{
			var provider = new StatementsAnalyzer();
			return provider.Calculate(node);
		}

		private static int CalculateLogicalComplexity(MemberNode node)
		{
			var provider = new LogicalComplexityAnalyzer();
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

		private IEnumerable<TypeCoupling> CalculateClassCoupling(MemberNode node)
		{
			var provider = new MemberClassCouplingAnalyzer(Model);
			return provider.Calculate(node);
		}

		private IEnumerable<MemberMetric> CalculateMemberMetrics(IEnumerable<MemberNode> nodes)
		{
			return from node in nodes
				   let analyzer = new HalsteadAnalyzer()
				   let halsteadMetrics = analyzer.Calculate(node)
				   where halsteadMetrics != null
				   let syntaxNode = node.SyntaxNode
				   let memberMetricKind = GetMemberMetricKind(node)
				   let source = CalculateClassCoupling(node)
				   let complexity = CalculateCyclomaticComplexity(node)
				   let logicalComplexity = CalculateLogicalComplexity(node)
				   let linesOfCode = CalculateLinesOfCode(node)
				   let numberOfParameters = CalculateNumberOfParameters(syntaxNode)
				   let numberOfLocalVariables = CalculateNumberOfLocalVariables(syntaxNode)
				   let maintainabilityIndex = CalculateMaintainablityIndex(complexity, linesOfCode, halsteadMetrics)
				   select new MemberMetric(
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