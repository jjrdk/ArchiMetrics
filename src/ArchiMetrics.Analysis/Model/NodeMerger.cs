// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NodeMerger.cs" company="Reimers.dk">
//   Copyright © Matthias Friedrich, Reimers.dk 2014
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the NodeMerger type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Model
{
	using System.Collections.Generic;
	using System.Linq;
	using Common.CodeReview;
	using Common.Structure;

    internal static class NodeMerger
	{
		public static IEnumerable<IModelNode> Merge(this IEnumerable<IModelNode> vertices)
		{
			return vertices.GroupBy(x => new { x.QualifiedName, x.Type })
				.Select(
					x =>
					new ModelNode(
						x.First().DisplayName, 
						x.Key.Type, 
						x.Select(y => y.Quality).GetQuality(), 
						x.Max(y => y.LinesOfCode), 
						x.Min(y => y.MaintainabilityIndex), 
						x.Max(y => y.CyclomaticComplexity), 
						x.SelectMany(y => y.Children).Merge().ToList()));
		}

		public static CodeQuality GetQuality(this IEnumerable<EvaluationResult> reviews)
		{
			return reviews.Select(x => x.Quality).GetQuality();
		}

		private static CodeQuality GetQuality(this IEnumerable<CodeQuality> input)
		{
			return input.Any() ? input.OrderBy(x => (int)x).First() : CodeQuality.Good;
		}
	}
}