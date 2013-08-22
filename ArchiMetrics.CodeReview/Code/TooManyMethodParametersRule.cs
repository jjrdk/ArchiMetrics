// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TooManyMethodParametersRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the TooManyMethodParametersRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Code
{
	using ArchiMetrics.Common;
	using ArchiMetrics.Common.CodeReview;
	using Roslyn.Compilers.CSharp;

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

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var methodDeclaration = (MethodDeclarationSyntax)node;
			var parameterCount = methodDeclaration.ParameterList.Parameters.Count;

			if (parameterCount >= Limit)
			{
				return new EvaluationResult
						   {
							   Comment = "More than " + Limit + " parameters on method", 
							   ErrorCount = parameterCount, 
							   ImpactLevel = ImpactLevel.Member,
							   Quality = CodeQuality.NeedsReEngineering, 
							   QualityAttribute = QualityAttribute.Testability | QualityAttribute.Maintainability | QualityAttribute.Modifiability, 
							   Snippet = methodDeclaration.ToFullString()
						   };
			}

			return null;
		}
	}
}
