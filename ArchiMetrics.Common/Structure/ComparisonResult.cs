namespace ArchiMetrics.Common.Structure
{
	using System.Collections.Generic;
	using System.Linq;

	public class ComparisonResult
	{
		public ComparisonResult(ComparisonResultKind kind, IModelNode pattern, params IModelNode[] matches)
			: this(kind, pattern, matches == null ? null : matches.AsEnumerable())
		{
		}

		public ComparisonResult(ComparisonResultKind kind, IModelNode pattern, IEnumerable<IModelNode> matches)
		{
			Kind = kind;
			Pattern = pattern;
			Matches = matches == null ? null : matches.ToArray();
		}

		public ComparisonResultKind Kind { get; private set; }

		public IModelNode Pattern { get; private set; }

		public IEnumerable<IModelNode> Matches { get; private set; }
	}
}