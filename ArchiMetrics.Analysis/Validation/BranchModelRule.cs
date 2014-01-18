// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BranchModelRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the IModelRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Validation
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using ArchiMetrics.Common.Structure;

	internal class BranchModelRule : IModelRule
	{
		private readonly IModelNode _pattern;

		public BranchModelRule(IModelNode pattern)
		{
			_pattern = pattern;
		}

		public Task<IEnumerable<IValidationResult>> Validate(IModelNode modelTree)
		{
			return Task.Factory.StartNew(
				() =>
				{
					var result = Contains(modelTree, _pattern);
					return new IValidationResult[] { new BranchResult(result.Kind == ComparisonResultKind.Same, _pattern) }.AsEnumerable();
				});
		}

		private static ComparisonResult Contains(IModelNode tree, IModelNode pattern)
		{
			var deepComparison = tree.Flatten().Select(x => Compare(x, pattern)).ToArray();
			if (deepComparison.Any(x => x.Kind == ComparisonResultKind.Same))
			{
				return new ComparisonResult(ComparisonResultKind.Same, pattern, deepComparison.Where(x => x.Kind == ComparisonResultKind.Same).SelectMany(x => x.Matches).Distinct());
			}

			if (deepComparison.Any(x => x.Kind == ComparisonResultKind.Partial))
			{
				return new ComparisonResult(ComparisonResultKind.Partial, pattern, deepComparison.Where(x => x.Kind == ComparisonResultKind.Partial).SelectMany(x => x.Matches).Distinct());
			}

			return new ComparisonResult(ComparisonResultKind.Different, pattern, null);
		}

		private static ComparisonResult Compare(IModelNode vertex, IModelNode pattern)
		{
			if (vertex.QualifiedName.EndsWith(pattern.QualifiedName))
			{
				var node = (IModelNode)vertex;
				var patternNode = (IModelNode)pattern;
				if (!node.Children.Any() && !patternNode.Children.Any())
				{
					return new ComparisonResult(ComparisonResultKind.Same, pattern, vertex);
				}

				if (!patternNode.Children.Any())
				{
					return new ComparisonResult(ComparisonResultKind.Partial, pattern, vertex);
				}

				var childComparisons = node.Children.Zip(patternNode.Children, Compare).ToArray();
				return childComparisons.All(x => x.Kind == ComparisonResultKind.Same)
						   ? new ComparisonResult(ComparisonResultKind.Same, pattern, vertex)
						   : childComparisons.All(x => x.Kind != ComparisonResultKind.Different)
								 ? new ComparisonResult(ComparisonResultKind.Partial, pattern, vertex)
								 : new ComparisonResult(ComparisonResultKind.Different, pattern, null);
			}

			return new ComparisonResult(ComparisonResultKind.Different, pattern, null);
		}
	}
}