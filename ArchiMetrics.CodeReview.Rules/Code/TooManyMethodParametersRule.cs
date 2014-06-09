// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TooManyMethodParametersRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the TooManyMethodParametersRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Rules.Code
{
	using ArchiMetrics.Common.CodeReview;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;

	internal class TooManyMethodParametersRule : CodeEvaluationBase
	{
		private const int Limit = 5;

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
				return "More than " + Limit + " parameters on method";
			}
		}

		public override string Suggestion
		{
			get
			{
				return "Refactor method to reduce number of dependencies passed.";
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

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var methodDeclaration = (MethodDeclarationSyntax)node;
			var parameterCount = methodDeclaration.ParameterList.Parameters.Count;

			if (parameterCount >= Limit)
			{
				return new EvaluationResult
						   {
							   ErrorCount = parameterCount, 
							   Snippet = methodDeclaration.ToFullString()
						   };
			}

			return null;
		}
	}
}
