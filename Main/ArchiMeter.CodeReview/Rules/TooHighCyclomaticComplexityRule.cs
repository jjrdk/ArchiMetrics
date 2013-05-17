// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TooHighCyclomaticComplexityRule.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the TooHighCyclomaticComplexityRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMeter.CodeReview.Rules
{
	using Common;
	using Common.Metrics;
	using Metrics;
	using Roslyn.Compilers.CSharp;

	internal class TooHighCyclomaticComplexityRule : EvaluationBase
	{
		private const int Limit = 8;
		readonly CyclomaticComplexityAnalyzer analyzer = new CyclomaticComplexityAnalyzer();

		public override SyntaxKind EvaluatedKind
		{
			get { return SyntaxKind.MethodDeclaration; }
		}

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var methodDeclaration = (MethodDeclarationSyntax)node;
			var complexity = analyzer.Calculate(new MemberNode(string.Empty, string.Empty, MemberKind.Method, 0, methodDeclaration));
			if (complexity >= Limit)
			{
				return new EvaluationResult
						   {
							   Comment = "Method too complex.", 
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