// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ComparisonResult.cs" company="Reimers.dk">
//   Copyright © Matthias Friedrich, Reimers.dk 2014
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ComparisonResult type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Common.Structure
{
    using System.Collections.Generic;
    using System.Linq;

    internal class ComparisonResult
	{
		public ComparisonResult(ComparisonResultKind kind, IModelNode pattern, params IModelNode[] matches)
			: this(kind, pattern, matches?.AsEnumerable())
		{
		}

		public ComparisonResult(ComparisonResultKind kind, IModelNode pattern, IEnumerable<IModelNode> matches)
		{
			Kind = kind;
			Pattern = pattern;
			Matches = matches?.AsArray();
		}

		public ComparisonResultKind Kind { get; private set; }

		public IModelNode Pattern { get; private set; }

		public IEnumerable<IModelNode> Matches { get; private set; }
	}
}