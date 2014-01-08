// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TooLowMaintainabilityIndexRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the TooLowMaintainabilityIndexRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Rules.Semantic
{
	using ArchiMetrics.Analysis.Metrics;
	using ArchiMetrics.Common.CodeReview;
	using Roslyn.Compilers.Common;
	using Roslyn.Compilers.CSharp;
	using Roslyn.Services;

	internal class TooLowMaintainabilityIndexRule : SemanticEvaluationBase
	{
		public TooLowMaintainabilityIndexRule()
		{
			Threshold = 40;
		}

		public override SyntaxKind EvaluatedKind
		{
			get
			{
				return SyntaxKind.MethodDeclaration;
			}
		}

		public override string Title
		{
			get
			{
				return "Method Unmaintainable";
			}
		}

		public override string Suggestion
		{
			get
			{
				return "Refactor method to improve maintainability.";
			}
		}

		public override CodeQuality Quality
		{
			get
			{
				return CodeQuality.NeedsRefactoring;
			}
		}

		public override QualityAttribute QualityAttribute
		{
			get
			{
				return QualityAttribute.Testability | QualityAttribute.Maintainability | QualityAttribute.Modifiability;
			}
		}

		public override ImpactLevel ImpactLevel
		{
			get
			{
				return ImpactLevel.Member;
			}
		}

		public int Threshold { get; set; }

		protected override EvaluationResult EvaluateImpl(SyntaxNode node, ISemanticModel semanticModel, ISolution solution)
		{
			var counter = new MemberMetricsCalculator(semanticModel);

			var methodDeclaration = (MethodDeclarationSyntax)node;
			var metric = counter.Calculate(methodDeclaration);
			if (metric.MaintainabilityIndex <= Threshold)
			{
				var snippet = node.ToFullString();
				return new EvaluationResult
				{
					ErrorCount = 1, 
					Snippet = snippet
				};
			}

			return null;
		}
	}
}