// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnusedCodeRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the UnusedCodeRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Semantic
{
	using System.Linq;
	using System.Threading;
	using ArchiMetrics.Common;
	using ArchiMetrics.Common.CodeReview;
	using Roslyn.Compilers.Common;
	using Roslyn.Compilers.CSharp;
	using Roslyn.Services;

	internal abstract class UnusedCodeRule : SemanticEvaluationBase
	{
		protected override EvaluationResult EvaluateImpl(SyntaxNode node, ISemanticModel semanticModel, ISolution solution)
		{
			var symbol = semanticModel.GetDeclaredSymbol(node);
			var callers = symbol.FindCallers(solution, CancellationToken.None);

			if (!callers.Any())
			{
				return new EvaluationResult
					   {
						   Comment = "Uncalled code detected.", 
						   ImpactLevel = ImpactLevel.Member, 
						   Namespace = GetCompilationUnitNamespace(node.GetLocation().SourceTree.GetRoot()), 
						   Quality = CodeQuality.Incompetent, 
						   QualityAttribute = QualityAttribute.CodeQuality, 
						   Snippet = node.ToFullString()
					   };
			}

			return null;
		}
	}
}
