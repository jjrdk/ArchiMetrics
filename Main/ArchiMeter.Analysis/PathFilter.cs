namespace ArchiMeter.Analysis
{
	using System;
	using Common;

	public class PathFilter
	{
		private readonly Func<ProjectDefinition, bool> _filter;

		public PathFilter(Func<ProjectDefinition, bool> filter)
		{
			_filter = filter;
		}

		public bool Filter(ProjectDefinition definition)
		{
			return _filter(definition);
		}
	}
}