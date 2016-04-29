// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PossibleStaticMethod.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the PossibleStaticMethod type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Rules.Semantic
{
	using System.Threading.Tasks;
	using Analysis.Common.CodeReview;
	using ArchiMetrics.Analysis;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;

	internal class PossibleStaticMethod : SemanticEvaluationBase
	{
		public override string ID
		{
			get
			{
				return "AM0057";
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
				return "Method Can Be Made Static";
			}
		}

		public override string Suggestion
		{
			get
			{
				return "Mark method as static.";
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

		protected override Task<EvaluationResult> EvaluateImpl(
			SyntaxNode node, 
			SemanticModel semanticModel, 
			Solution solution)
		{
			var method = (MethodDeclarationSyntax)node;
			var analyzer = new SemanticAnalyzer(semanticModel);

			if (analyzer.CanBeMadeStatic(method))
			{
				var snippet = method.ToFullString();
				return Task.FromResult(
					new EvaluationResult
					{
						Snippet = snippet
					});
			}

			return Task.FromResult((EvaluationResult)null);
		}
	}
}