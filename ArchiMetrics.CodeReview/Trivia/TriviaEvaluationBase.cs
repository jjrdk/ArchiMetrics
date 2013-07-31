// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TriviaEvaluationBase.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the TriviaEvaluationBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Trivia
{
	using Code;
	using Common;
	using Roslyn.Compilers.CSharp;

	internal abstract class TriviaEvaluationBase : EvaluationBase, ITriviaEvaluation
	{
		public EvaluationResult Evaluate(SyntaxTrivia node)
		{
			var result = EvaluateImpl(node);
			if (result != null)
			{
				var sourceTree = node.GetLocation().SourceTree;
				var filePath = sourceTree.FilePath;
				var unitNamespace = GetCompilationUnitNamespace(sourceTree.GetRoot());
				result.Namespace = unitNamespace;
				result.FilePath = filePath;
				result.LinesOfCodeAffected = 0;
			}

			return result;
		}

		protected abstract EvaluationResult EvaluateImpl(SyntaxTrivia node);
	}
}
