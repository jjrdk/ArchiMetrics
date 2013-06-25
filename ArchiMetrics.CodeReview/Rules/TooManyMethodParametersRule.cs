// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TooManyMethodParametersRule.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the TooManyMethodParametersRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMetrics.CodeReview.Rules
{
	using Common;
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
							   Quality = CodeQuality.NeedsReEngineering, 
							   QualityAttribute = QualityAttribute.Testability | QualityAttribute.Maintainability | QualityAttribute.Modifiability, 
							   Snippet = methodDeclaration.ToFullString()
						   };
			}

			return null;
		}
	}
}
