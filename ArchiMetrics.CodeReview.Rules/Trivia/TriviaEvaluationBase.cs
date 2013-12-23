// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TriviaEvaluationBase.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the TriviaEvaluationBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Rules.Trivia
{
	using ArchiMetrics.CodeReview.Rules.Code;
	using ArchiMetrics.Common.CodeReview;
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
				var typeDefinition = GetNodeType(node.Token.Parent);
				var unitNamespace = GetNamespace(node.Token.Parent);
				if (result.ErrorCount == 0)
				{
					result.ErrorCount = 1;
				}

				result.LinesOfCodeAffected = 0;
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

		protected abstract EvaluationResult EvaluateImpl(SyntaxTrivia node);
	}
}
