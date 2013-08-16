// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TooLowMaintainabilityIndexRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the TooLowMaintainabilityIndexRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Semantic
{
	using ArchiMetrics.Analysis.Metrics;
	using ArchiMetrics.CodeReview.Code;
	using ArchiMetrics.Common;
	using Roslyn.Compilers.Common;
	using Roslyn.Compilers.CSharp;
	using Roslyn.Services;

	internal class TooLowMaintainabilityIndexRule : EvaluationBase, ISemanticEvaluation
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

		public int Threshold { get; set; }

		public EvaluationResult Evaluate(SyntaxNode node, ISemanticModel semanticModel, ISolution solution)
		{
			if (semanticModel == null)
			{
				return null;
			}

			var counter = new MemberMetricsCalculator(semanticModel);

			var methodDeclaration = (MethodDeclarationSyntax)node;
			var metric = counter.Calculate(methodDeclaration);
			if (metric.MaintainabilityIndex <= Threshold)
			{
				return new EvaluationResult
					   {
						   Comment = "Possible unmaintainable method.",
						   ErrorCount = 1,
						   Quality = CodeQuality.NeedsRefactoring,
						   QualityAttribute = QualityAttribute.Testability | QualityAttribute.Maintainability | QualityAttribute.Modifiability,
						   Snippet = node.ToFullString()
					   };
			}

			return null;
		}
	}
}