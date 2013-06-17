namespace ArchiMeter.Analysis
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using ArchiMeter.Common;

	public class DependencyChain
	{
		private readonly EdgeItem[] _chain;

		public DependencyChain(IEnumerable<EdgeItem> referenceChain, EdgeItem root, EdgeItem lastEdge)
		{
			_chain = referenceChain.Concat(new[] { lastEdge }).ToArray();
			Length = _chain.Length;
			Root = root;
			LastEdge = lastEdge;
			IsCircular = root.Dependant == lastEdge.Dependency;
			Name = GetName();
		}

		public string Name { get; private set; }

		public IEnumerable<EdgeItem> ReferenceChain
		{
			get { return _chain; }
		}

		public EdgeItem Root { get; private set; }

		public EdgeItem LastEdge { get; private set; }

		public int Length { get; private set; }

		public bool IsCircular { get; private set; }

		public bool IsContinuation(EdgeItem edge)
		{
			return LastEdge.Dependency == edge.Dependant && !(ReferenceChain.Any(e => e.Dependant == edge.Dependant));
		}

		public bool Contains(EdgeItem edge)
		{
			return ReferenceChain.Any(e => e.Dependant == edge.Dependant && e.Dependency == edge.Dependency);
		}

		private string GetName()
		{
			var firstLink = ReferenceChain.Select(e => e.Dependant).OrderBy(x => x).FirstOrDefault();
			var startIndex = Array.FindIndex(_chain, e => e.Dependant == firstLink);
			var startSeq = ReferenceChain.Skip(startIndex);
			var endSeq = ReferenceChain.Take(startIndex);
			return string.Join(" --> ", startSeq.Concat(endSeq));
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
	}
}
