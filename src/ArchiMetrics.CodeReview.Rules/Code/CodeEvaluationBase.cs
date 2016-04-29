// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CodeEvaluationBase.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the CodeEvaluationBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Rules.Code
{
    using Analysis.Common.CodeReview;
    using Microsoft.CodeAnalysis;

	internal abstract class CodeEvaluationBase : EvaluationBase, ICodeEvaluation
	{
		public EvaluationResult Evaluate(SyntaxNode node)
		{
			var result = EvaluateImpl(node);
			if (result != null)
			{
				var sourceTree = node.GetLocation().SourceTree;
				var filePath = sourceTree.FilePath;
				var typeDefinition = GetNodeType(node);
				var unitNamespace = GetNamespace(node);
				if (result.ErrorCount == 0)
				{
					result.ErrorCount = 1;
				}

				if (result.LinesOfCodeAffected <= 0)
				{
					result.LinesOfCodeAffected = GetLinesOfCode(node);
				}

				result.Namespace = unitNamespace;
				result.TypeKind = typeDefinition.Item1;
				result.TypeName = typeDefinition.Item2;
				result.Title = Title;
				result.Suggestion = Suggestion;
				result.Quality = Quality;
				result.QualityAttribute = QualityAttribute;
				result.ImpactLevel = ImpactLevel;
				result.FilePath = filePath;
			}

			return result;
		}

		protected abstract EvaluationResult EvaluateImpl(SyntaxNode node);
	}
}
