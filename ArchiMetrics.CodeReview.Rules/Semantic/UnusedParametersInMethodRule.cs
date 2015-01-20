// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnusedParametersInMethodRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the UnusedParametersInMethodRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Rules.Semantic
{
	using System.Linq;
	using System.Threading.Tasks;
	using ArchiMetrics.Analysis;
	using ArchiMetrics.Common.CodeReview;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;

	internal class UnusedParametersInMethodRule : SemanticEvaluationBase
	{
		public override string ID
		{
			get
			{
				return "AM0066";
			}
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
				return "Unused Parameter in Method";
			}
		}

		public override string Suggestion
		{
			get
			{
				return "Removed unused parameter.";
			}
		}

		public override CodeQuality Quality
		{
			get
			{
				return CodeQuality.NeedsReview;
			}
		}

		public override QualityAttribute QualityAttribute
		{
			get
			{
				return QualityAttribute.CodeQuality;
			}
		}

		public override ImpactLevel ImpactLevel
		{
			get
			{
				return ImpactLevel.Member;
			}
		}

		protected override Task<EvaluationResult> EvaluateImpl(SyntaxNode node, SemanticModel semanticModel, Solution solution)
		{
			var method = (MethodDeclarationSyntax)node;
			var analyzer = new SemanticAnalyzer(semanticModel);
			if (analyzer.GetUnusedParameters(method).Any())
			{
				var snippet = method.ToFullString();
				return Task.FromResult(new EvaluationResult
					   {
						   Snippet = snippet
					   });
			}

			return Task.FromResult((EvaluationResult)null);
		}
	}
}