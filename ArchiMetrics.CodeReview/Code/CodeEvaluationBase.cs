// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CodeEvaluationBase.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the CodeEvaluationBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Code
{
	using ArchiMetrics.Common;
	using ArchiMetrics.Common.CodeReview;
	using Roslyn.Compilers.CSharp;

	internal abstract class CodeEvaluationBase : EvaluationBase, ICodeEvaluation
	{
		public EvaluationResult Evaluate(SyntaxNode node)
		{
			var result = EvaluateImpl(node);
			if (result != null)
			{
				var sourceTree = node.GetLocation().SourceTree;
				var filePath = sourceTree.FilePath;
				var unitNamespace = GetCompilationUnitNamespace(sourceTree.GetRoot());
				result.Namespace = unitNamespace;
				result.FilePath = filePath;
				result.LinesOfCodeAffected = GetLinesOfCode(result.Snippet);
			}

			return result;
		}

		protected abstract EvaluationResult EvaluateImpl(SyntaxNode node);
	}
}
