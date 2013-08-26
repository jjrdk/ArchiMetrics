// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TooManyMethodParametersRule.cs" company="Reimers.dk">
//   Copyright � Reimers.dk 2012
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

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var methodDeclaration = (MethodDeclarationSyntax)node;
			var parameterCount = methodDeclaration.ParameterList.Parameters.Count;

			if (parameterCount >= Limit)
			{
				return new EvaluationResult
						   {
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
