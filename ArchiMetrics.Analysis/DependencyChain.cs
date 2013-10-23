// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DependencyChain.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the DependencyChain type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using ArchiMetrics.Common.Structure;

	public class DependencyChain
	{
		private readonly EdgeItemBase[] _chain;

		public DependencyChain(IEnumerable<EdgeItemBase> referenceChain, EdgeItemBase root, EdgeItemBase lastEdge)
		{
			_chain = referenceChain.Concat(new[] { lastEdge }).ToArray();
			Length = _chain.Length;
			Root = root;
			LastEdge = lastEdge;
			IsCircular = root.Dependant == lastEdge.Dependency;
			Name = GetName();
		}

		public string Name { get; private set; }

		public IEnumerable<EdgeItemBase> ReferenceChain
		{
			get { return _chain; }
		}

		public EdgeItemBase Root { get; private set; }

		public EdgeItemBase LastEdge { get; private set; }

		public int Length { get; private set; }

		public bool IsCircular { get; private set; }

		public bool IsContinuation(EdgeItemBase edge)
		{
			return LastEdge.Dependency == edge.Dependant && ReferenceChain.All(e => e.Dependant != edge.Dependant);
		}

		public bool Contains(MetricsEdgeItem metricsEdge)
		{
			return ReferenceChain.Any(e => e.Dependant == metricsEdge.Dependant && e.Dependency == metricsEdge.Dependency);
		}

		public override string ToString()
		{
			return Name;
		}

		public override int GetHashCode()
		{
			return Name.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return obj != null && obj.GetHashCode() == GetHashCode();
		}

		private string GetName()
		{
			var firstLink = ReferenceChain.Select(e => e.Dependant).OrderBy(x => x).FirstOrDefault();
			var startIndex = Array.FindIndex(_chain, e => e.Dependant == firstLink);
			var startSeq = ReferenceChain.Skip(startIndex);
			var endSeq = ReferenceChain.Take(startIndex);
			return string.Join(" --> ", startSeq.Concat(endSeq));
		}
	}
}
