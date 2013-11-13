// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SemanticEvaluationBase.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the SemanticEvaluationBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Rules.Semantic
{
	using ArchiMetrics.CodeReview.Rules.Code;
	using ArchiMetrics.Common.CodeReview;
	using Roslyn.Compilers.Common;
	using Roslyn.Compilers.CSharp;
	using Roslyn.Services;

	internal abstract class SemanticEvaluationBase : EvaluationBase, ISemanticEvaluation
	{
		public EvaluationResult Evaluate(SyntaxNode node, ISemanticModel semanticModel, ISolution solution)
		{
			var result = EvaluateImpl(node, semanticModel, solution);
			if (result != null)
			{
				var sourceTree = node.GetLocation().SourceTree;
				var filePath = sourceTree.FilePath;
				if (string.IsNullOrWhiteSpace(result.Namespace))
				{
					var unitNamespace = GetCompilationUnitNamespace(sourceTree.GetRoot());
					result.Namespace = unitNamespace;
				}

				result.FilePath = filePath;
				result.LinesOfCodeAffected = GetLinesOfCode(result.Snippet);
			}

			return result;
		}

		protected abstract EvaluationResult EvaluateImpl(SyntaxNode node, ISemanticModel semanticModel, ISolution solution);
	}
}