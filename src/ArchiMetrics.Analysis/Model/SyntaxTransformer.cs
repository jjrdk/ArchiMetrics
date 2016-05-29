// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyntaxTransformer.cs" company="Reimers.dk">
//   Copyright © Matthias Friedrich, Reimers.dk 2014
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the SyntaxTransformer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Model
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using Common;
	using Common.Structure;

    internal class SyntaxTransformer : TransformerBase, ISyntaxTransformer
	{
		public Task<IEnumerable<IModelNode>> Transform(IEnumerable<IModelNode> source, IEnumerable<TransformRule> rules, CancellationToken cancellationToken)
		{
			return Task.Factory.StartNew(
				() =>
				{
					var allRules = rules.AsArray();
					var transformedVertices = from vertex in source
											  select TransformVertexRecursive(vertex, allRules);

					return transformedVertices.AsArray().AsEnumerable();
				}, 
				cancellationToken);
		}

		private IModelNode TransformVertexRecursive(IModelNode vertex, TransformRule[] transforms)
		{
			var newName = transforms.Aggregate(
				vertex.DisplayName, 
				(name, rule) => GetTransform(rule.Pattern).Replace(name, rule.Name ?? string.Empty));
			var type = vertex.Type;
			var linesOfCode = vertex.LinesOfCode;
			var maintainabilityIndex = vertex.MaintainabilityIndex;
			var cyclomaticComplexity = vertex.CyclomaticComplexity;
			var quality = vertex.Quality;
			var children = vertex.Children.Select(x => TransformVertexRecursive(x, transforms));
			return vertex is StaticModelNode
					   ? new StaticModelNode(
							 newName, 
							 type, 
							 quality, 
							 linesOfCode, 
							 maintainabilityIndex, 
							 cyclomaticComplexity, 
							 children.ToList())
					   : new ModelNode(
							 newName, 
							 type, 
							 quality, 
							 linesOfCode, 
							 maintainabilityIndex, 
							 cyclomaticComplexity, 
							 children.ToList());
		}
	}
}