// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ComparisonResult.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ComparisonResult type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Common.Structure
{
	using System.Collections.Generic;
	using System.Linq;

	internal class ComparisonResult
	{
		public ComparisonResult(ComparisonResultKind kind, IModelNode pattern, params IModelNode[] matches)
			: this(kind, pattern, matches == null ? null : matches.AsEnumerable())
		{
		}

		public ComparisonResult(ComparisonResultKind kind, IModelNode pattern, IEnumerable<IModelNode> matches)
		{
			Kind = kind;
			Pattern = pattern;
			Matches = matches == null ? null : matches.AsArray();
		}

		public ComparisonResultKind Kind { get; private set; }

		public IModelNode Pattern { get; private set; }

		public IEnumerable<IModelNode> Matches { get; private set; }
	}
}